using System;
using System.Linq;
using System.Web.Http;
using Model;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using libNS_NhanVien;
using System.Collections.Generic;
using static libNS_NhanVien.ClassNS_NhanVien;
using libQuy_HoaDon;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using libDM_DoiTuong;
using System.IO;
using banhang24.Hellper;
using banhang24.Models;
using Model.Service.common;
using System.Net.Http.Headers;
using System.Web.Script.Services;
using System.Threading.Tasks;
using Model.Infrastructure;
using banhang24.Compress;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class NS_NhanVienAPIController : BaseApiController
    {
        #region GET
        public IEnumerable<Object> GetNS_NhanVien_InforBasic(Guid idDonVi)
        {
            using (var _db = SystemDBContext.GetDBContext())
            {
                return new ClassNS_NhanVien(_db).SP_GetStaffWorking(idDonVi).Select(x => new
                {
                    x.ID,
                    x.MaNhanVien,
                    x.TenNhanVien,
                    SoDienThoai = x.DienThoaiDiDong,
                    x.GioiTinh,
                    URLAnh = x.Image
                });
            }
        }

        public IHttpActionResult GetNhanVien(string id, string iddv)
        {
            try
            {
                using (SsoftvnContext db = new SsoftvnContext(id))
                {
                    ClassNS_NhanVien cnv = new ClassNS_NhanVien(db);
                    List<GetListNhanVienDatLichCheckinResult> result = cnv.GetListNhanVienDatLich(new Guid(iddv));
                    return ActionTrueData(result);
                }
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        public IEnumerable<Object> GetNS_NhanVien_DaTaoND(Guid idDonVi)
        {
            using (var _db = SystemDBContext.GetDBContext())
            {
                return new ClassNS_NhanVien(_db).SP_GetStaffWorkingND(idDonVi);
            }
        }

        public IEnumerable<Object> GetListDoiTuong()
        {
            //return ClassNS_NhanVien.Gets(null);
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var lst = db.DM_DoiTuong;
                if (lst != null)
                {
                    return lst.Select(x => new
                    {
                        ID = x.ID,
                        TenNhanVien = x.TenDoiTuong,
                        MaNhanVien = x.MaDoiTuong,
                        SoDienThoai = x.DienThoai
                    }).ToList();
                }
                else
                    return null;
            }
        }

        public IEnumerable<Object> GetListNhanVienChuaCoND(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                var data = db.Database.SqlQuery<NS_NhanVienDTO>("exec GetListNhanVienChuaCoND @ID_ChiNhanh", paramlist.ToArray());
                return data.ToList();
            }
        }

        public IEnumerable<Object> GetListNhanVienEdit(Guid idnhanvien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_NhanVien", idnhanvien));
                var listTon = db.Database.SqlQuery<NS_NhanVien>("exec GetListNhanVienEdit @ID_NhanVien", paramlist.ToArray());
                return listTon.ToList();
            }
        }

        [HttpGet]
        public IHttpActionResult GetNhanVien_CoBangLuong(DateTime fromDate, DateTime toDate, Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    NhanSuService _NhanSuService = new NhanSuService(db);
                    var data = _NhanSuService.GetNhanVien_CoBangLuong(fromDate, toDate, idDonVi);
                    return ActionTrueData(data);
                }
                catch (Exception e)
                {
                    return Exeption(e);
                }
            }
        }

        /// <summary>
        /// Get list nhan vien {ID, MaNhanVien, TenNhanVien, ID_NguoiDung} all Chi Nhanh
        /// (NS_NhanVien left join NS_QuaTrinhCongTac, HT_NguoiDung)
        /// </summary>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        [DeflateCompression]
        public IHttpActionResult GetNhanVien_NguoiDung()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NS_NhanVien> lstNV = new List<NS_NhanVien>();
                if (db != null)
                {
                    try
                    {
                        var data = (from nv in db.NS_NhanVien
                                    join qt in db.NS_QuaTrinhCongTac on nv.ID equals qt.ID_NhanVien into NV_QT
                                    from nv_qt in NV_QT.DefaultIfEmpty()
                                    join nd in db.HT_NguoiDung on nv.ID equals nd.ID_NhanVien into NV_ND
                                    from nv_nd in NV_ND.DefaultIfEmpty()
                                    join nva in (from nvag in db.NS_NhanVien_Anh
                                                 group nvag by nvag.ID_NhanVien into nvagg
                                                 select new
                                                 {
                                                     ID_NhanVien = nvagg.FirstOrDefault().ID_NhanVien,
                                                     URLAnh = nvagg.FirstOrDefault().URLAnh
                                                 }) on nv.ID equals nva.ID_NhanVien into NV_NVA
                                    from nv_nva in NV_NVA.DefaultIfEmpty()
                                        // get staff not delete and is working
                                    where (nv.TrangThai == null || nv.TrangThai == 1) && (nv.DaNghiViec == false)
                                    //where nv_qt.ID_DonVi == idDonVi || (nv_qt.ID_DonVi == Guid.Empty) // if ID_DonVi == null
                                    select new
                                    {
                                        ID = nv.ID,
                                        MaNhanVien = nv.MaNhanVien,
                                        SoDienThoai = nv.DienThoaiDiDong,
                                        TenNhanVien = nv.TenNhanVien,
                                        GioiTinh = nv.GioiTinh,
                                        TenNhanVienKhongDau = nv.TenNhanVienKhongDau,
                                        TenNhanVienChuCaiDau = nv.TenNhanVienChuCaiDau,
                                        ID_NguoiDung = nv_nd.ID == null ? Guid.Empty : nv_nd.ID,
                                        ID_DonVi = nv_qt.ID_DonVi == null ? Guid.Empty : nv_qt.ID_DonVi,
                                        URLAnh = nv_nva.URLAnh == null ? "" : nv_nva.URLAnh
                                    }).ToList().Distinct();
                        return Json(new { res = true, data });
                    }
                    catch (Exception e)
                    {
                        CookieStore.WriteLog("NS_NhanVienAPI_GetNhanVien_NguoiDung: " + e.InnerException + e.Message);
                        return Json(new { res = false, mes = "NS_NhanVienAPI_GetNhanVien_NguoiDung: " + e.InnerException + e.Message });
                    }
                }
                return Json(new { res = false });
            }
        }
        /// <summary>
        /// Get cookies of user login { ID,ID_NhanVien,TaiKhoan}
        /// </summary>
        /// <returns></returns>
        public HT_NguoiDung GetUserCookies()
        {
            return banhang24.Hellper.contant.GetUserCookies();
        }

        // chiet khau
        public List<NhanVienChiTietDTO> GetChiTietNhanVien(string id)
        {
            try
            {
                using (var _db = SystemDBContext.GetDBContext())
                {

                    return new classNS_NhanVien_ChiTiet(_db).SelectChiTiet(id).ToList();
                }

            }
            catch { return null; }

        }

        public IHttpActionResult GetListAddNewStaffUnit()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                return Json(db.HT_CauHinhPhanMem.Where(o => o.ThongTinChiTietNhanVien == true).Select(o => o.ID_DonVi).ToList());
            }
        }

        public List<SP_ChietKhauNV> GetChietKhauNV_byIDQuiDoi(string idQuiDoi, Guid idChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                return new ClassNS_NhanVien(db).SP_GetListChietKhauNhanVien_By_IDQuiDoi(idQuiDoi, idChiNhanh);
            }
        }

        /// <summary>
        /// get all Chiet khau of all NhanVien, all ChiNhanh
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Object> GetAllChietKhau_AllNV()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (db != null)
                    {
                        var data = from x in db.ChietKhauMacDinh_NhanVien
                                   select new
                                   {
                                       ID_DonVi = x.ID_DonVi,
                                       ID_NhanVien = x.ID_NhanVien,
                                       TenNhanVien = x.NS_NhanVien.TenNhanVien,
                                       ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                       ChietKhau_ThucHien = x.ChietKhau, // chiet khau thuc hien
                                       LaPhanTram_ThucHien = x.LaPhanTram, // la chiet khau thuc hien
                                       ChietKhau_YeuCau = x.ChietKhau_YeuCau,
                                       LaPhanTram_YeuCau = x.LaPhanTram_YeuCau,
                                       ChietKhau_TuVan = x.ChietKhau_TuVan,
                                       LaPhanTram_TuVan = x.LaPhanTram_TuVan,
                                   };
                        return data.ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("NS_NhanVienAPI_GetAllChietKhau_AllNV: " + ex.Message + ex.InnerException);
                return null;
            }
        }

        [HttpPost, HttpGet]
        [Compress.DeflateCompression]
        /// <summary>
        /// get danh sach chiet khau theo hang hoa cua nhan vien
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetChietKhauHangHoaNVien_byChiNhanh(Guid idChiNhanh)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (db != null)
                    {
                        var data = (from x in db.ChietKhauMacDinh_NhanVien
                                    where x.ID_DonVi == idChiNhanh
                                    select new
                                    {
                                        x.ID_DonVi,
                                        x.ID_NhanVien,
                                        x.NS_NhanVien.TenNhanVien,
                                        x.ID_DonViQuiDoi,
                                        ChietKhau_ThucHien = x.ChietKhau, // chiet khau thuc hien
                                        LaPhanTram_ThucHien = x.LaPhanTram, // la chiet khau thuc hien
                                        x.ChietKhau_YeuCau,
                                        x.LaPhanTram_YeuCau,
                                        x.ChietKhau_TuVan,
                                        x.LaPhanTram_TuVan,
                                        x.LaPhanTram_BanGoi,
                                        x.ChietKhau_BanGoi,
                                        x.TheoChietKhau_ThucHien,
                                    }).ToList();
                        return Json(new { res = true, data });
                    }
                    else
                    {
                        return Json(new { res = false, mes = "DB null" });
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Concat("GetChietKhauHangHoaNVien_byChiNhanh ", ex.Message, ex.InnerException);
                CookieStore.WriteLog(err);
                return Json(new { res = false, mes = err });
            }
        }

        [HttpGet, HttpPost]
        public bool AddChietKhau_ByIDNhom(libNS_NhanVien.Param_ChietKhauNhomHang lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                return new classNS_NhanVien_ChiTiet(db).SP_AddChietKhau_ByIDNhom(lstParam);
            }
        }
        #region ChietKhau HoaDon
        [HttpPost]
        public IHttpActionResult Get_ChietKhauHoaDon_byNhanVien(Param_GetChietKhauHoaDon lstParam)
        {
            string strIns = string.Empty;

            try
            {
                var idNhanVien = lstParam.ID_NhanVien;
                var currentPage = lstParam.CurrentPage;
                var pageSize = lstParam.PageSize;

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var data = from hd in db.ChietKhauMacDinh_HoaDon
                               join ct in db.ChietKhauMacDinh_HoaDon_ChiTiet on hd.ID equals ct.ID_ChietKhauHoaDon
                               where ct.ID_NhanVien == idNhanVien
                               && hd.TrangThai != 0
                               group new { hd, ct }
                               by new
                               {
                                   ID_NhanVien = ct.ID_NhanVien,
                                   ID = hd.ID,
                                   ID_DonVi = hd.ID_DonVi,
                                   GiaTriChietKhau = hd.GiaTriChietKhau,
                                   TinhChietKhauTheo = hd.TinhChietKhauTheo,
                                   ChungTuApDung = hd.ChungTuApDung,
                                   GhiChu = hd.GhiChu,
                               }
                           into g
                               select new
                               {
                                   ID = g.Key.ID,
                                   ID_DonVi = g.Key.ID_DonVi,
                                   GiaTriChietKhau = g.Key.GiaTriChietKhau,
                                   TinhChietKhauTheo = g.Key.TinhChietKhauTheo,
                                   LaPhanTram = g.Key.TinhChietKhauTheo == 3 ? false : true,
                                   ChungTuApDung = g.Key.ChungTuApDung,
                                   GhiChu = g.Key.GhiChu,
                               };

                    var totalRecord = data.Count();
                    return Json(new
                    {
                        res = true,
                        mess = strIns,
                        DataSoure = data.OrderBy(x => x.GiaTriChietKhau).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList(),
                        TotalRecord = totalRecord,
                        TotalPage = System.Math.Ceiling(totalRecord * 1.0 / lstParam.PageSize),
                    });
                }
            }
            catch (Exception ex)
            {
                strIns = "Get_ChietKhauHoaDon_ChiTiet_byID: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = strIns });
            }
        }

        [HttpPost]
        public IHttpActionResult Get_ChietKhauHoaDon_byDonVi(Param_GetChietKhauHoaDon lstParam)
        {
            string strIns = string.Empty;

            try
            {
                var idNhanVien = lstParam.ID_NhanVien;
                var idDonvi = lstParam.ID_DonVi;
                var currentPage = lstParam.CurrentPage;
                var pageSize = lstParam.PageSize;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var data = new classNS_NhanVien_ChiTiet(db).SP_Get_ChietKhauHoaDon_byDonVi(idDonvi);
                    // filter by role: view (todo)

                    var totalRecord = data.Count();
                    if (pageSize != 0)
                    {
                        data = data.OrderBy(x => x.GiaTriChietKhau).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    }
                    foreach (SP_GetChietKhauHoaDon item in data)
                    {
                        item.NhanViens = new classNS_NhanVien_ChiTiet(db).SP_GetChietKhauHoaDon_ChiTiet_byID(item.ID, idDonvi ?? Guid.Empty);
                    }

                    return Json(new
                    {
                        res = true,
                        mess = strIns,
                        DataSoure = data,
                        //AllDataDource = data,
                        TotalRecord = totalRecord,
                        TotalPage = System.Math.Ceiling(totalRecord * 1.0 / lstParam.PageSize),
                    });
                }
            }
            catch (Exception ex)
            {
                strIns = "Get_ChietKhauHoaDon_byDonVi: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = strIns });
            }
        }

        public IHttpActionResult Get_ChietKhauHoaDon_ChiTiet_byID(Guid idChietKhau, Guid idDonVi)
        {
            string strIns = string.Empty;

            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var data = new classNS_NhanVien_ChiTiet(db).SP_GetChietKhauHoaDon_ChiTiet_byID(idChietKhau, idDonVi);
                    return Json(new
                    {
                        res = true,
                        mess = strIns,
                        DataSoure = data,
                    });
                }
            }
            catch (Exception ex)
            {
                strIns = "Get_ChietKhauHoaDon_ChiTiet_byID: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = strIns });
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult Add_ChietKhauHoaDon([FromBody] JObject data)
        {
            string strIns = string.Empty;

            try
            {
                ChietKhauMacDinh_HoaDon objNew = data.ToObject<ChietKhauMacDinh_HoaDon>();

                // add ChietKhauHoaDon
                ChietKhauMacDinh_HoaDon dataAdd = new ChietKhauMacDinh_HoaDon { };
                dataAdd.ID = Guid.NewGuid();
                dataAdd.ID_DonVi = objNew.ID_DonVi;
                dataAdd.TinhChietKhauTheo = objNew.TinhChietKhauTheo;
                dataAdd.GiaTriChietKhau = objNew.GiaTriChietKhau;
                dataAdd.ChungTuApDung = objNew.ChungTuApDung;
                dataAdd.GhiChu = objNew.GhiChu;
                dataAdd.TrangThai = 1;
                dataAdd.NgayTao = DateTime.Now;

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    db.ChietKhauMacDinh_HoaDon.Add(dataAdd);
                    // add ChietKhauHoaDon_ChiTiet
                    foreach (ChietKhauMacDinh_HoaDon_ChiTiet item in objNew.NhanViens)
                    {
                        ChietKhauMacDinh_HoaDon_ChiTiet nvien = new ChietKhauMacDinh_HoaDon_ChiTiet();
                        nvien.ID = Guid.NewGuid();
                        nvien.ID_NhanVien = item.ID;
                        nvien.ID_ChietKhauHoaDon = dataAdd.ID;
                        db.ChietKhauMacDinh_HoaDon_ChiTiet.Add(nvien);
                    }
                    db.SaveChanges();
                }
                if (strIns == string.Empty)
                {
                    return Json(new { res = true, mess = strIns, DataSoure = new { dataAdd.ID } });
                }
                else
                {
                    return Json(new { res = false, mess = strIns });
                }
            }
            catch (Exception ex)
            {
                strIns = "Add_ChietKhauHoaDon: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = strIns });
            }
        }

        [HttpPut, HttpPost]
        public IHttpActionResult Update_ChietKhauHoaDon([FromBody] JObject data)
        {
            string sErr = string.Empty;

            try
            {
                // add ChietKhauHoaDon
                ChietKhauMacDinh_HoaDon objData = data.ToObject<ChietKhauMacDinh_HoaDon>();
                var id = objData.ID;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                    sErr = _classNS_NhanVien_ChiTiet.Update_ChietKhauHoaDon(objData);

                    // delete ChiTiet_ChietKhauHoaDopn and add again
                    if (sErr == string.Empty)
                    {
                        sErr = _classNS_NhanVien_ChiTiet.Delete_ChietKhauHoaDon_ChiTiet(id);

                        // add ChietKhauHoaDon_ChiTiet
                        foreach (ChietKhauMacDinh_HoaDon_ChiTiet item in objData.NhanViens)
                        {
                            ChietKhauMacDinh_HoaDon_ChiTiet nvien = new ChietKhauMacDinh_HoaDon_ChiTiet();
                            nvien.ID = Guid.NewGuid();
                            nvien.ID_NhanVien = item.ID;
                            nvien.ID_ChietKhauHoaDon = id;
                            sErr = _classNS_NhanVien_ChiTiet.Add_ChietKhauHoaDon_ChiTiet(nvien);
                        }
                    }
                    if (sErr == string.Empty)
                    {
                        return Json(new { res = true, mess = sErr, DataSoure = new { objData.ID } });
                    }
                    else
                    {
                        return Json(new { res = false, mess = sErr });
                    }
                }
            }
            catch (Exception ex)
            {
                sErr = "Update_ChietKhauHoaDon: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = sErr });
            }
        }

        [HttpPut, HttpPost]
        public IHttpActionResult Delete_ChietKhauHoaDon(Guid id)
        {
            string sErr = string.Empty;

            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    sErr = new classNS_NhanVien_ChiTiet(db).Delete_ChietKhauHoaDon(id);
                    if (sErr == string.Empty)
                    {
                        return Json(new { res = true, mess = string.Empty });
                    }
                    else
                    {
                        return Json(new { res = false, mess = sErr });
                    }
                }
            }
            catch (Exception ex)
            {
                sErr = "Delete_ChietKhauHoaDon: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = sErr });
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult CheckExist_ChietKhauHD_NhanVien(Param_GetChietKhauHoaDon lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var data = new classNS_NhanVien_ChiTiet(db).CheckExist_ChietKhauHD_NhanVien(lstParam);
                if (data == null)
                {
                    return Json(new { res = false });
                }
                else
                {
                    return Json(new { res = true, DataSoure = data });
                }
            }
        }

        #endregion

        #region ChietKhau DoanhThu
        [HttpPost]
        public IHttpActionResult Get_ChietKhauDoanhThu_byDonVi(Param_GetChietKhauHoaDon lstParam)
        {
            string strIns = string.Empty;

            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                    var idNhanVien = lstParam.ID_NhanVien;
                    var idDonvi = lstParam.ID_DonVi;
                    var currentPage = lstParam.CurrentPage;
                    var pageSize = lstParam.PageSize;

                    List<SP_GetChietKhauDoanhThu> data = _classNS_NhanVien_ChiTiet.SP_Get_ChietKhauDoanhThu_byDonVi(lstParam);
                    int? totalRow = 0; double? totalPage = 0;
                    if (data.Count > 0)
                    {
                        totalRow = data[0].TotalRow;
                        totalPage = data[0].TotalPage;
                    }
                    foreach (SP_GetChietKhauDoanhThu item in data)
                    {
                        item.DoanhThuChiTiet = _classNS_NhanVien_ChiTiet.SP_GetChietKhauDoanhThuChiTiet_byID(item.ID);
                        item.NhanViens = _classNS_NhanVien_ChiTiet.SP_GetChietKhauDoanhThuNhanVien_byID(item.ID, idDonvi ?? Guid.Empty);
                    }

                    return Json(new
                    {
                        res = true,
                        mess = strIns,
                        DataSoure = data,
                        TotalRecord = totalRow,
                        TotalPage = totalPage,
                    });
                }
            }
            catch (Exception ex)
            {
                strIns = "Get_ChietKhauDoanhThu_byDonVi: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = strIns });
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult Add_ChietKhauDoanhThu([FromBody] JObject data)
        {
            string strIns = string.Empty;

            try
            {
                // add ChietKhauHoaDon
                ChietKhauDoanhThu objNew = data.ToObject<ChietKhauDoanhThu>();

                ChietKhauDoanhThu dataAdd = new ChietKhauDoanhThu { };
                dataAdd.ID = Guid.NewGuid();
                dataAdd.ID_DonVi = objNew.ID_DonVi;
                dataAdd.TinhChietKhauTheo = objNew.TinhChietKhauTheo;
                dataAdd.ApDungTuNgay = objNew.ApDungTuNgay;
                dataAdd.ApDungDenNgay = objNew.ApDungDenNgay;
                dataAdd.GhiChu = objNew.GhiChu;
                dataAdd.TrangThai = 1;
                dataAdd.LoaiNhanVienApDung = objNew.LoaiNhanVienApDung;
                dataAdd.NgayTao = DateTime.Now;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                    strIns = _classNS_NhanVien_ChiTiet.Add_ChietKhauDoanhThu(dataAdd);

                    // add ChietKhauDoanhThu_ChiTiet
                    foreach (ChietKhauDoanhThu_ChiTiet item in objNew.DoanhThuChiTiet)
                    {
                        ChietKhauDoanhThu_ChiTiet ct = new ChietKhauDoanhThu_ChiTiet();
                        ct.ID = Guid.NewGuid();
                        ct.ID_ChietKhauDoanhThu = dataAdd.ID;
                        ct.DoanhThuTu = item.DoanhThuTu;
                        ct.DoanhThuDen = item.DoanhThuDen;
                        ct.GiaTriChietKhau = item.GiaTriChietKhau;
                        ct.LaPhanTram = item.LaPhanTram;
                        strIns = _classNS_NhanVien_ChiTiet.Add_ChietKhauDoanhThu_ChiTiet(ct);
                    }

                    // add ChietKhauDoanhThu_NhanVien
                    foreach (ChietKhauDoanhThu_NhanVien item in objNew.NhanViens)
                    {
                        ChietKhauDoanhThu_NhanVien nvien = new ChietKhauDoanhThu_NhanVien();
                        nvien.ID = Guid.NewGuid();
                        nvien.ID_ChietKhauDoanhThu = dataAdd.ID;
                        nvien.ID_NhanVien = item.ID;
                        strIns = _classNS_NhanVien_ChiTiet.Add_ChietKhauDoanhThu_NhanVien(nvien);
                    }

                    if (strIns == string.Empty)
                    {
                        return Json(new { res = true, mess = strIns, DataSoure = new { dataAdd.ID } });
                    }
                    else
                    {
                        return Json(new { res = false, mess = strIns });
                    }
                }
            }
            catch (Exception ex)
            {
                strIns = "Add_ChietKhauDoanhThu: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = strIns });
            }
        }

        [HttpPut, HttpPost]
        public IHttpActionResult Update_ChietKhauDoanhThu([FromBody] JObject data)
        {
            string sErr = string.Empty;

            try
            {
                // add ChietKhauHoaDon
                ChietKhauDoanhThu objData = data.ToObject<ChietKhauDoanhThu>();
                var id = objData.ID;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                    sErr = _classNS_NhanVien_ChiTiet.Update_ChietKhauDoanhThu(objData);

                    if (sErr == string.Empty)
                    {
                        // delete
                        sErr = _classNS_NhanVien_ChiTiet.Delete_ChietKhauDoanhThu_ChiTiet(id);

                        // add again ChiTiet
                        foreach (ChietKhauDoanhThu_ChiTiet item in objData.DoanhThuChiTiet)
                        {
                            ChietKhauDoanhThu_ChiTiet ct = new ChietKhauDoanhThu_ChiTiet();
                            ct.ID = Guid.NewGuid();
                            ct.ID_ChietKhauDoanhThu = id;
                            ct.DoanhThuTu = item.DoanhThuTu;
                            ct.DoanhThuDen = item.DoanhThuDen;
                            ct.GiaTriChietKhau = item.GiaTriChietKhau;
                            ct.LaPhanTram = item.LaPhanTram;
                            sErr = _classNS_NhanVien_ChiTiet.Add_ChietKhauDoanhThu_ChiTiet(ct);
                        }

                        // delete
                        sErr = _classNS_NhanVien_ChiTiet.Delete_ChietKhauDoanhThu_NhanVien(id);

                        // add again NhanVien
                        foreach (ChietKhauDoanhThu_NhanVien item in objData.NhanViens)
                        {
                            ChietKhauDoanhThu_NhanVien nvien = new ChietKhauDoanhThu_NhanVien();
                            nvien.ID = Guid.NewGuid();
                            nvien.ID_ChietKhauDoanhThu = id;
                            nvien.ID_NhanVien = item.ID;
                            sErr = _classNS_NhanVien_ChiTiet.Add_ChietKhauDoanhThu_NhanVien(nvien);
                        }
                    }
                    if (sErr == string.Empty)
                    {
                        return Json(new { res = true, mess = sErr, DataSoure = new { objData.ID } });
                    }
                    else
                    {
                        return Json(new { res = false, mess = sErr });
                    }
                }
            }
            catch (Exception ex)
            {
                sErr = "Update_ChietKhauDoanhThu: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = sErr });
            }
        }

        [HttpPut, HttpPost]
        public IHttpActionResult Delete_ChietKhauDoanhThu(Guid id)
        {
            string sErr = string.Empty;

            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    sErr = new classNS_NhanVien_ChiTiet(db).Delete_ChietKhauDoanhThu(id);
                }
                if (sErr == string.Empty)
                {
                    return Json(new { res = true, mess = string.Empty });
                }
                else
                {
                    return Json(new { res = false, mess = sErr });
                }
            }
            catch (Exception ex)
            {
                sErr = "Delete_ChietKhauDoanhThu: " + ex.Message + ex.InnerException;
                return Json(new { res = false, mess = sErr });
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult CheckExist_ChietKhauDT_NhanVien(Param_GetChietKhauHoaDon lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var data = new classNS_NhanVien_ChiTiet(db).CheckExist_ChietKhauDoanhThu_NhanVien(lstParam);
                if (data == null || data.Count() == 0)
                {
                    return Json(new { res = true });
                }
                else
                {
                    return Json(new { res = false, DataSoure = data });
                }
            }
        }
        #endregion

        [HttpGet]
        public bool AddChiTiet(Guid id, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                return new classNS_NhanVien_ChiTiet(db).AddChiTietByIDNhom(id, idnhanvien, iddonvi);
            }
        }
        [HttpGet]
        public bool AddChiTietbyMaHH(string maHH, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                return new classNS_NhanVien_ChiTiet(db).AddChiTietBymaHH(maHH, idnhanvien, iddonvi);
            }
        }

        [HttpGet]
        public void deleteAllChietKhau(Guid ID_NhanVien, Guid? ID_NhomHang, string maHH, Guid? ID_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                List<NhanVienChiTietDTO> lstAllGBs = _classNS_NhanVien_ChiTiet.GetNhanVien_NhomHang(ID_NhanVien, ID_NhomHang, maHH, ID_DonVi);
                for (int i = 0; i < lstAllGBs.Count(); i++)
                {
                    Guid id = lstAllGBs[i].ID;
                    _classNS_NhanVien_ChiTiet.deleteChiTietbyID(id);
                }
            }
        }

        [HttpGet]
        public bool deleteChiTiet(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                return new classNS_NhanVien_ChiTiet(db).deleteChiTietbyID(id);
            }
        }

        [HttpPost, ActionName("PutChietKhauChiTiet")]
        public IHttpActionResult PutChietKhauChiTiet([FromBody] JObject data)
        {
            //Guid id = data["id"].ToObject<Guid>();
            List<NhanVien_ChiTiet_UpdateNhanVien> objList = data["objData"].ToObject<List<NhanVien_ChiTiet_UpdateNhanVien>>();
            string strUpd = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                foreach (var item in objList)
                {
                    strUpd = _classNS_NhanVien_ChiTiet.Update_ChietKhauCT(item.ID, item.ChietKhau, item.LaPhanTram);
                }
            }
            if (strUpd != null && strUpd != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPost, ActionName("PutYeuCauChiTiet")]
        public IHttpActionResult PutYeuCauChiTiet([FromBody] JObject data)
        {
            //Guid id = data["id"].ToObject<Guid>();
            List<NhanVien_ChiTiet_UpdateNhanVien> objList = data["objData"].ToObject<List<NhanVien_ChiTiet_UpdateNhanVien>>();
            string strUpd = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                foreach (var item in objList)
                {
                    strUpd = _classNS_NhanVien_ChiTiet.Update_YeuCauCT(item.ID, item.YeuCau, item.LaPhanTram_YeuCau);
                }
            }
            if (strUpd != null && strUpd != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }


        [HttpPost, ActionName("PutTuVanChiTiet")]
        public IHttpActionResult PutTuVanChiTiet([FromBody] JObject data)
        {
            //Guid id = data["id"].ToObject<Guid>();
            List<NhanVien_ChiTiet_UpdateNhanVien> objList = data["objData"].ToObject<List<NhanVien_ChiTiet_UpdateNhanVien>>();
            string strUpd = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classNS_NhanVien_ChiTiet _classNS_NhanVien_ChiTiet = new classNS_NhanVien_ChiTiet(db);
                foreach (var item in objList)
                {
                    strUpd = _classNS_NhanVien_ChiTiet.Update_TuVanCT(item.ID, item.TuVan, item.LaPhanTram_TuVan);
                }
            }
            if (strUpd != null && strUpd != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        public List<ListLHPages> getAllPage<T>(List<T> lstLHs, float sohang)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            if (lstLHs != null)
            {
                int dem = 1;
                float SoTrang = lstLHs.Count / sohang;
                for (int i = 0; i < SoTrang; i++)
                {
                    ListLHPages LH_page = new ListLHPages();
                    LH_page.SoTrang = dem;
                    listPage.Add(LH_page);
                    dem = dem + 1;
                }
                return listPage;
            }
            else
            {
                return null;
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetListNhanVienNhomHang(Param_ChietKhauNhomHang lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classNS_NhanVien_ChiTiet classNhanVienChiTiet = new classNS_NhanVien_ChiTiet(db);
                    List<ChietKhauMacDinh_NhanVienPRC> data = classNhanVienChiTiet.GetCaiDatChietKhau_HangHoa(lstParam);
                    return Json(new
                    {
                        res = true,
                        LstData = data,
                        Rowcount = data.Count() > 0 ? data[0].TotalRow : 0,
                        numberPage = data.Count() > 0 ? data[0].TotalPage : 0,
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        res = false,
                        mes = ex,
                    });
                }
            }
        }
        public int getNumber_Page(float sohang, int pageSize)
        {
            if (sohang > 0)
            {
                float SoTrang = sohang / pageSize;
                if (SoTrang > (int)SoTrang)
                    return (int)SoTrang + 1;
                else
                    return (int)SoTrang;
            }
            else
            {
                return 0;
            }
        }
        //xuất file
        [HttpGet, HttpPost]
        public string ExportExcelChietKhau_NhanVien(Param_ChietKhauNhomHang lstParam, string columnsHide, string TenChiNhanh, string TenNhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string fileSave = string.Empty;
                string Times = "Ngày xuất: " + DateTime.Now.ToString("dd/MM/yyyy");
                try
                {
                    classNS_NhanVien_ChiTiet classNhanVienChiTiet = new classNS_NhanVien_ChiTiet(db);
                    List<ChietKhauMacDinh_NhanVienPRC> lst = classNhanVienChiTiet.GetCaiDatChietKhau_HangHoa(lstParam);
                    Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                    DataTable excel = _Class_officeDocument.ToDataTable<ChietKhauMacDinh_NhanVienPRC>(lst);
                    excel.Columns.Remove("ID");
                    excel.Columns.Remove("IDQuyDoi");
                    excel.Columns.Remove("ID_NhomHang");
                    excel.Columns.Remove("TenHangHoa");
                    excel.Columns.Remove("TenDonViTinh");
                    excel.Columns.Remove("ThuocTinh_GiaTri");
                    excel.Columns.Remove("LaPTChietKhau");
                    excel.Columns.Remove("LaPTYeuCau");
                    excel.Columns.Remove("LaPTTuVan");
                    excel.Columns.Remove("LaPTBanGoi");
                    excel.Columns.Remove("TheoChietKhau_ThucHien");
                    excel.Columns.Remove("TotalRow");
                    excel.Columns.Remove("TotalPage");

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/Teamplate_ChietKhauHangHoaTheoNhanVien.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/ChietKhauHangHoaTheoNhanVien.xlsx");
                    fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                    _Class_officeDocument.listToOfficeExcelChiTiet_Stype(fileTeamplate, fileSave, excel, 5, 29, 24, false, columnsHide, Times, TenChiNhanh, TenNhanVien, 4);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                }
                catch (Exception ex)
                {
                    fileSave = ex.InnerException + ex.Message;
                }
                return fileSave;
            }
        }
        [HttpGet, ActionName("GetList_AllNhanVienNhomHang")]
        public System.Web.Http.Results.JsonResult<JsonResultExample<NhanVienChiTietDTO>> GetList_AllNhanVienNhomHang(Guid? id, Guid idNhanVien, string maHH, Guid? ID_DonVi, int nuberPage, int pageSize)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NhanVienChiTietDTO> lstAllGBs = new classNS_NhanVien_ChiTiet(db).GetNhanVien_NhomHang(idNhanVien, id, maHH, ID_DonVi);
                JsonResultExample<NhanVienChiTietDTO> jso = new JsonResultExample<NhanVienChiTietDTO>
                {
                    LstData = lstAllGBs
                };
                return Json(jso);
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult update_ChietKhau([FromBody] JObject data, double ChietKhau, bool LaPhanTram)
        {
            List<ChietKhauMacDinh_NhanVienPRC> objList = data["objData"].ToObject<List<ChietKhauMacDinh_NhanVienPRC>>();
            string strUpd = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var item in objList)
            {
                try
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        List<SqlParameter> parameter = new List<SqlParameter>();
                        parameter.Add(new SqlParameter("ID", item.ID));
                        parameter.Add(new SqlParameter("ChietKhau", ChietKhau));
                        parameter.Add(new SqlParameter("LaPhanTram", LaPhanTram));
                        db.Database.ExecuteSqlCommand("exec Update_ChietKhau_ByID @ID, @ChietKhau, @LaPhanTram", parameter.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    strUpd = ex.ToString();
                }
            }
            if (strUpd != null && strUpd != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult update_ChietKhau_BanGoi([FromBody] JObject data, double ChietKhau, bool LaPhanTram)
        {
            List<ChietKhauMacDinh_NhanVienPRC> objList = data["objData"].ToObject<List<ChietKhauMacDinh_NhanVienPRC>>();
            string strUpd = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var item in objList)
            {
                try
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        List<SqlParameter> parameter = new List<SqlParameter>();
                        parameter.Add(new SqlParameter("ID", item.ID));
                        parameter.Add(new SqlParameter("ChietKhau_BanGoi", ChietKhau));
                        parameter.Add(new SqlParameter("LaPhanTram_BanGoi", LaPhanTram));
                        db.Database.ExecuteSqlCommand("exec Update_ChietKhau_BanGoiByID @ID, @ChietKhau_BanGoi, @LaPhanTram_BanGoi", parameter.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    strUpd = ex.ToString();
                }
            }
            if (strUpd != null && strUpd != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult update_ChietKhau_YeuCau([FromBody] JObject data, double ChietKhau, bool LaPhanTram, int theoCKThucHien = 1)
        {
            List<ChietKhauMacDinh_NhanVienPRC> objList = data["objData"].ToObject<List<ChietKhauMacDinh_NhanVienPRC>>();
            string strUpd = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var item in objList)
            {
                try
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        List<SqlParameter> parameter = new List<SqlParameter>();
                        parameter.Add(new SqlParameter("ID", item.ID));
                        parameter.Add(new SqlParameter("ChietKhau_YeuCau", ChietKhau));
                        parameter.Add(new SqlParameter("LaPhanTram_YeuCau", LaPhanTram));
                        parameter.Add(new SqlParameter("TheoChietKhau_ThucHien", theoCKThucHien));
                        db.Database.ExecuteSqlCommand("exec Update_ChietKhau_YeuCauByID @ID, @ChietKhau_YeuCau, @LaPhanTram_YeuCau, @TheoChietKhau_ThucHien", parameter.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    strUpd = ex.ToString();
                }
            }
            if (strUpd != null && strUpd != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult update_ChietKhau_TuVan([FromBody] JObject data, double ChietKhau, bool LaPhanTram)
        {
            List<ChietKhauMacDinh_NhanVienPRC> objList = data["objData"].ToObject<List<ChietKhauMacDinh_NhanVienPRC>>();
            string strUpd = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var item in objList)
            {
                try
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        List<SqlParameter> parameter = new List<SqlParameter>();
                        parameter.Add(new SqlParameter("ID", item.ID));
                        parameter.Add(new SqlParameter("ChietKhau_TuVan", ChietKhau));
                        parameter.Add(new SqlParameter("LaPhanTram_TuVan", LaPhanTram));
                        db.Database.ExecuteSqlCommand("exec Update_ChietKhau_TuVanByID @ID, @ChietKhau_TuVan, @LaPhanTram_TuVan", parameter.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    strUpd = ex.ToString();
                }
            }
            if (strUpd != null && strUpd != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        [HttpGet]
        public string update_ChietKhau1(Guid ID, double ChietKhau, bool LaPhanTram)
        {
            string resurt = string.Empty;
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SqlParameter> parameter = new List<SqlParameter>();
                    parameter.Add(new SqlParameter("ID", ID));
                    parameter.Add(new SqlParameter("ChietKhau", ChietKhau));
                    parameter.Add(new SqlParameter("LaPhanTram", LaPhanTram));
                    db.Database.ExecuteSqlCommand("exec Update_ChietKhau_ByID @ID, @ChietKhau, @LaPhanTram", parameter.ToArray());
                }
                resurt = "CNTC";
            }
            catch
            {
                resurt = "";
            }
            return resurt;
        }
        [HttpGet]
        public string update_ChietKhau_YeuCau1(Guid ID, double ChietKhau, bool LaPhanTram)
        {
            string resurt = string.Empty;
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SqlParameter> parameter = new List<SqlParameter>();
                    parameter.Add(new SqlParameter("ID", ID));
                    parameter.Add(new SqlParameter("ChietKhau_YeuCau", ChietKhau));
                    parameter.Add(new SqlParameter("LaPhanTram_YeuCau", LaPhanTram));
                    db.Database.ExecuteSqlCommand("exec Update_ChietKhau_YeuCauByID @ID, @ChietKhau_YeuCau, @LaPhanTram_YeuCau", parameter.ToArray());
                }
                resurt = "CNTC";
            }
            catch
            {
                resurt = "";
            }
            return resurt;
        }
        [HttpGet]
        public string update_ChietKhau_TuVan1(Guid ID, double ChietKhau, bool LaPhanTram)
        {
            string resurt = string.Empty;
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SqlParameter> parameter = new List<SqlParameter>();
                    parameter.Add(new SqlParameter("ID", ID));
                    parameter.Add(new SqlParameter("ChietKhau_TuVan", ChietKhau));
                    parameter.Add(new SqlParameter("LaPhanTram_TuVan", LaPhanTram));
                    db.Database.ExecuteSqlCommand("exec Update_ChietKhau_TuVanByID @ID, @ChietKhau_TuVan, @LaPhanTram_TuVan", parameter.ToArray());
                }
                resurt = "CNTC";
            }
            catch
            {
                resurt = "";
            }
            return resurt;
        }
        [HttpPost]
        public IHttpActionResult SaoChep_CaiDatHoaHong(array_SaoChepChietKhau param)
        {
            try
            {
                string idNhanViens = string.Empty;
                if (param.arrID != null && param.arrID.Count > 0)
                {
                    idNhanViens = string.Join(",", param.arrID);
                }
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SqlParameter> parameter = new List<SqlParameter>();
                    parameter.Add(new SqlParameter("ID_DonVi", param.ID_DonVi));
                    parameter.Add(new SqlParameter("ID_NhanVien", param.ID_NhanVien));
                    parameter.Add(new SqlParameter("ID_NhanVien_new", idNhanViens));
                    parameter.Add(new SqlParameter("PhuongThuc", param.PhuongThuc));
                    db.Database.ExecuteSqlCommand("exec insert_SaoChepCaiDatHoaHong @ID_DonVi, @ID_NhanVien, @ID_NhanVien_new, @PhuongThuc", parameter.ToArray());
                }
                return ActionTrueData(string.Empty);
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException+ ex.Message);
            }
        }

        // nhan vien
        public List<NS_NhanVienSelect> GetNS_NhanVien()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassNS_NhanVien classnv = new ClassNS_NhanVien(db);
                IQueryable<NS_NhanVien> lstdata = classnv.Gets(null);
                List<NS_NhanVienSelect> lst = new List<NS_NhanVienSelect>();
                foreach (var item in lstdata)
                {
                    NS_NhanVienSelect select = new NS_NhanVienSelect();
                    select.ID = item.ID;
                    select.TenNhanVien = item.TenNhanVien;
                    lst.Add(select);
                }
                return lst;
            }
        }
        [HttpGet]
        public List<NS_NhanVienCaiDatChietKhau> getlistNhanVien_CaiDatChietKhau(Guid ID_DonVi, string MaNhanVien, string TrangThai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NS_NhanVienCaiDatChietKhau> lst = new List<NS_NhanVienCaiDatChietKhau>();
                string MaNV_search = "%%";
                string MaNV_search_TV = "%%";
                if (MaNhanVien != null && MaNhanVien != "")
                {
                    MaNV_search = "%" + CommonStatic.ConvertToUnSign(MaNhanVien.Trim()).ToLower() + "%";
                    MaNV_search_TV = "%" + MaNhanVien.Trim() + "%";
                }
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                sql.Add(new SqlParameter("Text_NhanVien", MaNV_search));
                sql.Add(new SqlParameter("Text_NhanVien_TV", MaNV_search_TV));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                lst = db.Database.SqlQuery<NS_NhanVienCaiDatChietKhau>("exec getlistNhanVien_CaiDatChietKhau @ID_DonVi, @Text_NhanVien, @Text_NhanVien_TV, @TrangThai", sql.ToArray()).ToList();
                return lst;
            }
        }
        // GET: api/NS_NhanVienAPI/5
        [ResponseType(typeof(NS_NhanVien))]
        public IHttpActionResult GetNS_NhanVien(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var sImg = string.Empty;
                var lstImg = db.NS_NhanVien_Anh.Where(x => x.ID_NhanVien == id).Select(x => x.URLAnh).ToList();
                if (lstImg != null && lstImg.Count() > 0)
                {
                    sImg = lstImg.FirstOrDefault().ToString();
                }
                var data = db.NS_NhanVien.Where(x => x.ID == id).Select(x => new NS_NhanVienBK
                {
                    ID = x.ID,
                    TenNhanVien = x.TenNhanVien,
                    MaNhanVien = x.MaNhanVien,
                    DienThoaiDiDong = x.DienThoaiDiDong,
                    Email = x.Email,
                    ThuongTru = x.ThuongTru,
                    NguyenQuan = x.NguyenQuan,
                    SoBHXH = x.SoBHXH,
                    SoCMND = x.SoCMND,
                    GhiChu = x.GhiChu,
                    NgaySinh = x.NgaySinh,
                    GioiTinh = x.GioiTinh,
                    DaNghiViec = x.DaNghiViec,
                    Image = sImg,
                }).ToList();
                if (data != null && data.Count() > 0)
                {
                    return Ok(data.FirstOrDefault());
                }
                else
                {
                    return NotFound();
                }
            }
        }

        public List<NS_NhanVienDTO> GetListNhanViens()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                IQueryable<NS_NhanVien> lstAllNVs = new ClassNS_NhanVien(db).Gets(null);
                List<NS_NhanVienDTO> lsrReturns = new List<NS_NhanVienDTO>();
                if (lstAllNVs != null && lstAllNVs.Count() > 0)
                {
                    foreach (NS_NhanVien item in lstAllNVs)
                    {

                        NS_NhanVienDTO itemData = new NS_NhanVienDTO
                        {
                            ID = item.ID,
                            TenNhanVien = item.TenNhanVien,
                            MaNhanVien = item.MaNhanVien,
                            DienThoaiDiDong = item.DienThoaiDiDong,
                            GioiTinh = item.GioiTinh,
                            SoBHXH = item.SoBHXH,
                            SoCMND = item.SoCMND,
                            GhiChu = item.GhiChu,
                            NgaySinh = item.NgaySinh,
                            ThuongTru = item.ThuongTru,
                            NguyenQuan = item.NguyenQuan,
                            Email = item.Email,
                            DaNghiViec = item.DaNghiViec
                        };
                        lsrReturns.Add(itemData);
                    }
                }
                return lsrReturns;
            }
        }
        public List<ListLHPages> getAllPagenew<T>(List<T> lstLHs, float sohang)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            if (lstLHs != null)
            {
                int dem = 1;
                float SoTrang = lstLHs.Count / sohang;
                for (int i = 0; i < SoTrang; i++)
                {
                    ListLHPages LH_page = new ListLHPages();
                    LH_page.SoTrang = dem;
                    listPage.Add(LH_page);
                    dem = dem + 1;
                }
                return listPage;
            }
            else
            {
                return null;
            }
        }

        public IHttpActionResult getListAllNhanViens(string maNhanVien, int trangthai, int pageSize, int pageNum)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NS_ReportNhanVien> lst_nv = new ClassNS_NhanVien(db).getListNhanViens(maNhanVien, trangthai);

                int Row = lst_nv.Count();
                List<ListLHPages> lstPage = getAllPagenew<NS_ReportNhanVien>(lst_nv, pageSize);
                List<NS_ReportNhanVien> lst = lst_nv.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
                JsonResultExample<NS_ReportNhanVien> json = new JsonResultExample<NS_ReportNhanVien>()
                {
                    Rowcount = Row,
                    LstData = lst,
                    LstPageNumber = lstPage
                };
                return Json(json);
            }
        }


        public int getRowAllNhanVien(string maNhanVien, int trangthai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NS_ReportNhanVien> lst = new ClassNS_NhanVien(db).getListNhanViens(maNhanVien, trangthai);
                if (lst != null)
                {
                    return lst.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        //trinhpv getListNhanVien_DonVi
        public List<NS_NhanVien_DonVi> getListNhanVien_DonVi(string ID_ChiNhanh, String nameNV)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NS_NhanVien_DonVi> lst = new ClassNS_NhanVien(db).getListNhanVien_DonVi(ID_ChiNhanh, nameNV);
                return lst;
            }
        }
        public List<ListLHPages> getPageAllNhanVien(string maNhanVien, int trangthai, float pageSize)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NS_ReportNhanVien> lstLHs = new ClassNS_NhanVien(db).getListNhanViens(maNhanVien, trangthai);
                if (lstLHs != null)
                {
                    int dem = 1;
                    float SoTrang = lstLHs.Count / pageSize;
                    for (int i = 0; i < SoTrang; i++)
                    {
                        ListLHPages LH_page = new ListLHPages();
                        LH_page.SoTrang = dem;
                        listPage.Add(LH_page);
                        dem = dem + 1;
                    }
                    return listPage;
                }
                else
                {
                    return null;
                }
            }
        }
        public List<NS_QuaTrinhCongTac_PRC> getlistQTCongTac(Guid ID_NhanVien)
        {
            List<NS_QuaTrinhCongTac_PRC> lst = new List<NS_QuaTrinhCongTac_PRC>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                lst = db.Database.SqlQuery<NS_QuaTrinhCongTac_PRC>("exec getList_QuaTrinhCongTacbyNhanVien @ID_NhanVien", sql.ToArray()).ToList();
                return lst;
            }
        }
        public List<HD_NhanVien> getListHD_NhanVien(Guid ID_NhanVien, int pageSize, int pageNum)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<HD_NhanVien> lst = new ClassNS_NhanVien(db).getlistHD_NhanVien(ID_NhanVien).Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
                return lst;
            }
        }
        //Lấy số bản ghi trong data getListHD_NhanVien
        public int getRowsHD_NhanVien(Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<HD_NhanVien> lstLHs = new ClassNS_NhanVien(db).getlistHD_NhanVien(ID_NhanVien);
                if (lstLHs != null)
                {
                    return lstLHs.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        //Lấy số trang hiển thị danh sách
        public List<ListLHPages> getPageHD_NhanVien(Guid ID_NhanVien, float pageSize)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<HD_NhanVien> lstLHs = new ClassNS_NhanVien(db).getlistHD_NhanVien(ID_NhanVien);
                if (lstLHs != null)
                {
                    int dem = 1;
                    float SoTrang = lstLHs.Count / pageSize;
                    for (int i = 0; i < SoTrang; i++)
                    {
                        ListLHPages LH_page = new ListLHPages();
                        LH_page.SoTrang = dem;
                        listPage.Add(LH_page);
                        dem = dem + 1;
                    }
                    return listPage;
                }
                else
                {
                    return null;
                }
            }
        }
        public System.Web.Http.Results.JsonResult<JsonResultExample<Quy_HoaDon_NhanVienPRC>> GetListQuyHoaDons(Guid? ID_NhanVien, int pageSize, int pageNum)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<Quy_HoaDon_NhanVienPRC> lst = new List<Quy_HoaDon_NhanVienPRC>();
                List<SqlParameter> parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                lst = db.Database.SqlQuery<Quy_HoaDon_NhanVienPRC>("exec getListSQ_NhanVien @ID_NhanVien", parameter.ToArray()).ToList();
                List<Quy_HoaDon_NhanVienPRC> lstSL = lst.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
                List<ListLHPages> lstPages = getAllPage<Quy_HoaDon_NhanVienPRC>(lst, pageSize);
                int rown = lst.Count();
                JsonResultExample<Quy_HoaDon_NhanVienPRC> jsonobj = new JsonResultExample<Quy_HoaDon_NhanVienPRC>
                {
                    Rowcount = rown,
                    LstPageNumber = lstPages,
                    LstData = lstSL
                };
                return Json(jsonobj);
            }

        }
        public int getRowsSQ_NhanVien(Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                IQueryable<Quy_HoaDon> lstAllHDs = new ClassNS_NhanVien(db).getListSQ_NhanVien(id => id.ID_NhanVien == ID_NhanVien);
                if (lstAllHDs != null)
                {
                    return lstAllHDs.Count();
                }
                else
                {
                    return 0;
                }
            }
        }
        public List<ListLHPages> getPageSQ_NhanVien(Guid ID_NhanVien, float pageSize)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                IQueryable<Quy_HoaDon> lstAllHDs = new ClassNS_NhanVien(db).getListSQ_NhanVien(id => id.ID_NhanVien == ID_NhanVien);
                if (lstAllHDs != null)
                {
                    int dem = 1;
                    float SoTrang = lstAllHDs.ToList().Count / pageSize;
                    for (int i = 0; i < SoTrang; i++)
                    {
                        ListLHPages LH_page = new ListLHPages();
                        LH_page.SoTrang = dem;
                        listPage.Add(LH_page);
                        dem = dem + 1;
                    }
                    return listPage;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region update
        // PUT: api/NS_NhanVienAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutNS_NhanVien(Guid id, NS_NhanVien NS_NhanVien)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string strUpd = new ClassNS_NhanVien(db).Update_NhanVien(NS_NhanVien);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }

        }

        // PUT: api/NS_NhanVienAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutNS_NhanVien([FromBody] JObject data, Guid ID_DonVi, Guid ID_NhanVien)
        {
            Guid id = data["id"].ToObject<Guid>();
            NS_NhanVien NS_NhanVien = data["objNVien"].ToObject<NS_NhanVien>();
            List<NS_QuaTrinhCongTac_PRC> objQuaTrinhCongTac = data["objQuaTrinhCongTac"].ToObject<List<NS_QuaTrinhCongTac_PRC>>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string chitiet_km = "Họ tên: " + NS_NhanVien.TenNhanVien;
            string noidung_km = ". Họ tên: " + NS_NhanVien.TenNhanVien;
            if (NS_NhanVien.NgaySinh != null)
            {
                noidung_km = noidung_km + ". Ngày sinh: " + NS_NhanVien.NgaySinh.Value.ToString("dd/MM/yyyy");
                chitiet_km = chitiet_km + "<br>Ngày sinh: " + NS_NhanVien.NgaySinh.Value.ToString("dd/MM/yyyy");
            }
            noidung_km = noidung_km + ". Giới tính: " + (NS_NhanVien.GioiTinh == true ? "Nam" : "Nữ") + ". Trạng thái: " + (NS_NhanVien.DaNghiViec == false ? "Đang làm việc" : "Đã nghỉ việc");
            chitiet_km = chitiet_km + "<br>Giới tính: " + (NS_NhanVien.GioiTinh == true ? "Nam" : "Nữ") + "<br>Trạng thái: " + (NS_NhanVien.DaNghiViec == false ? "Đang làm việc" : "Đã nghỉ việc");
            if (NS_NhanVien.SoCMND != "" & NS_NhanVien.SoCMND != null & NS_NhanVien.SoCMND != "null")
            {
                noidung_km = noidung_km + ". Số CMND: " + NS_NhanVien.SoCMND;
                chitiet_km = chitiet_km + "<br>Số CMND: " + NS_NhanVien.SoCMND;
            }
            if (NS_NhanVien.NguyenQuan != "" & NS_NhanVien.NguyenQuan != null & NS_NhanVien.NguyenQuan != "null")
            {
                noidung_km = noidung_km + ".Quê quán: " + NS_NhanVien.NguyenQuan;
                chitiet_km = chitiet_km + "<br>Quê quán: " + NS_NhanVien.NguyenQuan;
            }
            if (NS_NhanVien.ThuongTru != "" & NS_NhanVien.ThuongTru != null & NS_NhanVien.ThuongTru != "null")
            {
                noidung_km = noidung_km + ". Địa chỉ thường trú: " + NS_NhanVien.ThuongTru;
                chitiet_km = chitiet_km + "<br>Địa chỉ thường trú: " + NS_NhanVien.ThuongTru;
            }
            if (NS_NhanVien.DienThoaiDiDong != "" & NS_NhanVien.DienThoaiDiDong != null & NS_NhanVien.DienThoaiDiDong != "null")
            {
                noidung_km = noidung_km + ". Điện thoại: " + NS_NhanVien.DienThoaiDiDong;
                chitiet_km = chitiet_km + "<br>Điện thoại: " + NS_NhanVien.DienThoaiDiDong;
            }
            if (NS_NhanVien.Email != "" & NS_NhanVien.Email != null & NS_NhanVien.Email != "null")
            {
                noidung_km = noidung_km + ". Email: " + NS_NhanVien.Email;
                chitiet_km = chitiet_km + "<br>Email: " + NS_NhanVien.Email;
            }
            if (NS_NhanVien.SoBHXH != "" & NS_NhanVien.SoBHXH != null & NS_NhanVien.SoBHXH != "null")
            {
                noidung_km = noidung_km + ". Số BHXH: " + NS_NhanVien.SoBHXH;
                chitiet_km = chitiet_km + "<br>Số BHXH: " + NS_NhanVien.SoBHXH;
            }
            if (NS_NhanVien.GhiChu != "" & NS_NhanVien.GhiChu != null & NS_NhanVien.GhiChu != "null")
            {
                noidung_km = noidung_km + ". Ghi chú: " + NS_NhanVien.GhiChu;
                chitiet_km = chitiet_km + "<br>Ghi chú: " + NS_NhanVien.GhiChu;
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                string strUpd = _ClassNS_NhanVien.Update_NhanVien(NS_NhanVien);
                if (strUpd != null && strUpd != string.Empty)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                }
                else
                {
                    strUpd = _ClassNS_NhanVien.Delete_QuaTrinhCongTac(NS_NhanVien.ID);
                    string chinhanh = string.Empty;
                    string chinhanh_ct = string.Empty;
                    int k = 0;
                    foreach (var item in objQuaTrinhCongTac)
                    {
                        if (k == 0)
                        {
                            chinhanh = ". Chi nhánh làm việc: " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                            chinhanh_ct = "<br>Chi nhánh làm việc: " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                        }
                        else
                        {
                            chinhanh = chinhanh + ", " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                            chinhanh_ct = chinhanh_ct + ", " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                        }

                        Guid? ID_PhongBanDF = item.ID_PhongBan;
                        if (item.ID_PhongBan == null)
                        {
                            ID_PhongBanDF = db.NS_PhongBan.Where(x => x.ID_DonVi == null).FirstOrDefault().ID;
                        }
                        NS_QuaTrinhCongTac DM = new NS_QuaTrinhCongTac
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = NS_NhanVien.ID,
                            ID_DonVi = item.ID_ChiNhanh,
                            NgayApDung = DateTime.Now,
                            LaChucVuHienThoi = false,
                            LaDonViHienThoi = item.LaMacDinh,
                            // ID_PhongBan = item.ID_PhongBan
                            ID_PhongBan = ID_PhongBanDF

                        };
                        strUpd = _ClassNS_NhanVien.Add_QuaTrinhCongTac(DM);
                        k = k + 1;
                    }
                    noidung_km = noidung_km + chinhanh;
                    chitiet_km = chitiet_km + chinhanh_ct;
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "Nhân viên",
                        ThoiGian = DateTime.Now,
                        NoiDung = "Cập nhật nhân viên: " + NS_NhanVien.MaNhanVien + noidung_km,
                        NoiDungChiTiet = "Cập nhật nhân viên: <a style= \"cursor: pointer\" onclick = \"loadNhanVienbyMaKM('" + NS_NhanVien.MaNhanVien + "')\" >" + NS_NhanVien.MaNhanVien + "</a> <br>" + chitiet_km,
                        LoaiNhatKy = 2
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    return StatusCode(HttpStatusCode.NoContent);
                    //return CreatedAtRoute("DefaultApi", new { id = objNhanVien.ID }, objNhanVien);
                }
            }

        }

        public void Update_ChietKhauNhanVien(string stringSQL)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    new ClassNS_NhanVien(db).Update_ChietKhauNhanVien(stringSQL);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("NS_NhanVienAPI_Update_ChietKhauNhanVien: " + ex.InnerException + ex.Message);
            }
        }
        #endregion

        #region insert
        //import danh sách hàng hóa
        [HttpPost, HttpGet]
        public IHttpActionResult ImfortExcelChietKhau()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = _Class_officeDocument.CheckFileMau_ChietKhau(excelstream);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkExcel_ChietKhau(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult getList_DanhSachHangChietKhau(Guid ID_ChiNhanh, Guid ID_NhanVien)
        {
            string result = "";
            try
            {
                string str = string.Empty;
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            str = _Class_officeDocument.insert_ChietKhauMacDinhNhanVien(excelstream, ID_ChiNhanh, ID_NhanVien);
                        }
                    }
                }
                if (str != string.Empty)
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, str));

            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // POST: api/NS_NhanVienAPI
        [HttpPost, ActionName("PostNS_NhanVien1")]
        [ResponseType(typeof(NS_NhanVien))]
        public IHttpActionResult PostNS_NhanVien1(NS_NhanVien NS_NhanVien)
        {
            if (!ModelState.IsValid)
            {
                // return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            NS_NhanVien.ID = Guid.NewGuid();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string strIns = new ClassNS_NhanVien(db).Add_NhanVien(NS_NhanVien);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = NS_NhanVien.ID }, NS_NhanVien);
            }
        }

        [HttpPost, ActionName("PostNS_NhanVien")]
        [ResponseType(typeof(NS_NhanVien))]
        public IHttpActionResult PostNS_NhanVien([FromBody] JObject data, Guid ID_DonVi, Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                NS_NhanVien objNhanVien = data["objNVien"].ToObject<NS_NhanVien>();
                List<NS_QuaTrinhCongTac_PRC> objQuaTrinhCongTac = data["objQuaTrinhCongTac"].ToObject<List<NS_QuaTrinhCongTac_PRC>>();

                #region NhanVien
                string sMaNhanVien = string.Empty;
                string chitiet_km = "Họ tên: " + objNhanVien.TenNhanVien;
                string noidung_km = ". Họ tên: " + objNhanVien.TenNhanVien;
                if (objNhanVien.MaNhanVien == null)
                {
                    SqlParameter sql = new SqlParameter("MaNhanVien", "NV00001");
                    sMaNhanVien = db.Database.SqlQuery<string>("exec get_MaNhanVien @MaNhanVien", sql).FirstOrDefault();
                }
                else
                {
                    sMaNhanVien = objNhanVien.MaNhanVien;
                }
                objNhanVien.MaNhanVien = sMaNhanVien.Trim();
                NS_NhanVien itemNV = new NS_NhanVien();
                itemNV.ID = Guid.NewGuid();
                itemNV.MaNhanVien = sMaNhanVien.Trim();
                itemNV.TenNhanVien = objNhanVien.TenNhanVien;
                itemNV.NguoiTao = "ADMIN";
                itemNV.NgaySinh = objNhanVien.NgaySinh;
                itemNV.NgayTao = DateTime.Now;
                itemNV.GioiTinh = objNhanVien.GioiTinh;
                itemNV.DienThoaiDiDong = objNhanVien.DienThoaiDiDong;
                itemNV.ThuongTru = objNhanVien.ThuongTru;
                itemNV.Email = objNhanVien.Email;
                itemNV.NguyenQuan = objNhanVien.NguyenQuan;
                itemNV.SoBHXH = objNhanVien.SoBHXH;
                itemNV.SoCMND = objNhanVien.SoCMND;
                itemNV.GhiChu = objNhanVien.GhiChu;
                itemNV.DaNghiViec = objNhanVien.DaNghiViec;
                #endregion
                if (objNhanVien.NgaySinh != null)
                {
                    noidung_km = noidung_km + ". Ngày sinh: " + objNhanVien.NgaySinh.Value.ToString("dd/MM/yyyy");
                    chitiet_km = chitiet_km + "<br>Ngày sinh: " + objNhanVien.NgaySinh.Value.ToString("dd/MM/yyyy");
                }
                noidung_km = noidung_km + ". Giới tính: " + (objNhanVien.GioiTinh == true ? "Nam" : "Nữ") + ". Trạng thái: " + (objNhanVien.DaNghiViec == false ? "Đang làm việc" : "Đã nghỉ việc");
                chitiet_km = chitiet_km + "<br>Giới tính: " + (objNhanVien.GioiTinh == true ? "Nam" : "Nữ") + "<br>Trạng thái: " + (objNhanVien.DaNghiViec == false ? "Đang làm việc" : "Đã nghỉ việc");
                if (objNhanVien.SoCMND != "" & objNhanVien.SoCMND != null & objNhanVien.SoCMND != "null")
                {
                    noidung_km = noidung_km + ". Số CMND: " + objNhanVien.SoCMND;
                    chitiet_km = chitiet_km + "<br>Số CMND: " + objNhanVien.SoCMND;
                }
                if (objNhanVien.NguyenQuan != "" & objNhanVien.NguyenQuan != null & objNhanVien.NguyenQuan != "null")
                {
                    noidung_km = noidung_km + ".Quê quán: " + objNhanVien.NguyenQuan;
                    chitiet_km = chitiet_km + "<br>Quê quán: " + objNhanVien.NguyenQuan;
                }
                if (objNhanVien.ThuongTru != "" & objNhanVien.ThuongTru != null & objNhanVien.ThuongTru != "null")
                {
                    noidung_km = noidung_km + ". Địa chỉ thường trú: " + objNhanVien.ThuongTru;
                    chitiet_km = chitiet_km + "<br>Địa chỉ thường trú: " + objNhanVien.ThuongTru;
                }
                if (objNhanVien.DienThoaiDiDong != "" & objNhanVien.DienThoaiDiDong != null & objNhanVien.DienThoaiDiDong != "null")
                {
                    noidung_km = noidung_km + ". Điện thoại: " + objNhanVien.DienThoaiDiDong;
                    chitiet_km = chitiet_km + "<br>Điện thoại: " + objNhanVien.DienThoaiDiDong;
                }
                if (objNhanVien.Email != "" & objNhanVien.Email != null & objNhanVien.Email != "null")
                {
                    noidung_km = noidung_km + ". Email: " + objNhanVien.Email;
                    chitiet_km = chitiet_km + "<br>Email: " + objNhanVien.Email;
                }
                if (objNhanVien.SoBHXH != "" & objNhanVien.SoBHXH != null & objNhanVien.SoBHXH != "null")
                {
                    noidung_km = noidung_km + ". Số BHXH: " + objNhanVien.SoBHXH;
                    chitiet_km = chitiet_km + "<br>Số BHXH: " + objNhanVien.SoBHXH;
                }
                if (objNhanVien.GhiChu != "" & objNhanVien.GhiChu != null & objNhanVien.GhiChu != "null")
                {
                    noidung_km = noidung_km + ". Ghi chú: " + objNhanVien.GhiChu;
                    chitiet_km = chitiet_km + "<br>Ghi chú: " + objNhanVien.GhiChu;
                }
                string strIns = _ClassNS_NhanVien.Add_NhanVien(itemNV);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    string chinhanh = string.Empty;
                    string chinhanh_ct = string.Empty;
                    int k = 0;
                    foreach (var item in objQuaTrinhCongTac)
                    {
                        //DM_DonVi DV = ClassBH_HoaDon.getList_DonVibyID(item.ID_ChiNhanh);
                        if (k == 0)
                        {
                            chinhanh = ". Chi nhánh phòng ban làm việc: " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                            chinhanh_ct = "<br>Chi nhánh phòng ban làm việc: " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                        }
                        else
                        {
                            chinhanh = chinhanh + ", " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                            chinhanh_ct = chinhanh_ct + ", " + item.Text_ChiNhanh + " (" + item.Text_PhongBan + ")";
                        }
                        Guid? ID_PhongBanDF = item.ID_PhongBan;
                        if (ID_PhongBanDF == null)
                        {
                            ID_PhongBanDF = db.NS_PhongBan.Where(x => x.ID_DonVi == null).FirstOrDefault().ID;
                        }
                        NS_QuaTrinhCongTac DM = new NS_QuaTrinhCongTac
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = itemNV.ID,
                            ID_DonVi = item.ID_ChiNhanh,
                            NgayApDung = DateTime.Now,
                            LaChucVuHienThoi = false,
                            LaDonViHienThoi = item.LaMacDinh,
                            //ID_PhongBan = item.ID_PhongBan
                            ID_PhongBan = ID_PhongBanDF
                        };
                        strIns = _ClassNS_NhanVien.Add_QuaTrinhCongTac(DM);
                        k = k + 1;
                    }
                    noidung_km = noidung_km + chinhanh;
                    chitiet_km = chitiet_km + chinhanh_ct;
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "Nhân viên",
                        ThoiGian = DateTime.Now,
                        NoiDung = "Thêm mới nhân viên: " + sMaNhanVien.Trim() + noidung_km,
                        NoiDungChiTiet = "Thêm mới nhân viên: <a style= \"cursor: pointer\" onclick = \"loadNhanVienbyMaKM('" + sMaNhanVien.Trim() + "')\" >" + sMaNhanVien.Trim() + "</a> <br>" + chitiet_km,
                        LoaiNhatKy = 1
                    };
                    objNhanVien.ID = itemNV.ID;
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    return CreatedAtRoute("DefaultApi", new { id = objNhanVien.ID }, objNhanVien);
                }
            }
        }

        public string Post_ChietKhauNhanVien([FromBody] JObject data)
        {
            var strIns = string.Empty;
            try
            {
                List<ChietKhauMacDinh_NhanVien> lstChietKhau = data["lstChietKhau"].ToObject<List<ChietKhauMacDinh_NhanVien>>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    foreach (var item in lstChietKhau)
                    {
                        strIns = _ClassNS_NhanVien.Insert_ChietKhauMacDinh(item);
                    }
                }
            }
            catch (Exception ex)
            {
                strIns = "error";
                CookieStore.WriteLog("NS_NhanVienAPI_Post_ChietKhauNhanVien: " + ex.InnerException + ex.Message);
            }
            return strIns;
        }
        #endregion
        #region import nhân viên
        [HttpGet]
        public void Download_ThongTinNhanVien(string filePath, string fileSave)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                List<NS_PhongBan_PROC> lst = new List<NS_PhongBan_PROC>();
                List<SqlParameter> sql = new List<SqlParameter>();
                lst = db.Database.SqlQuery<NS_PhongBan_PROC>("exec getlistNS_PhongBan", sql.ToArray()).ToList();
                DataTable excel = _Class_officeDocument.ToDataTable<NS_PhongBan_PROC>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ImportExcel/NhanVien/" + filePath);
                string fileSave_path = HttpContext.Current.Server.MapPath("~/Template/ImportExcel/NhanVien/" + fileSave);
                _Class_officeDocument.listToOfficeExcel_ToSheet1(fileTeamplate, fileSave_path, excel, 3, 27, 24, true, null);
                HttpResponse Response = HttpContext.Current.Response;
                _Class_officeDocument.downloadFile(fileSave_path);
            }
        }
        // thông tin danh sách nhân viên todo
        [HttpPost]
        public IHttpActionResult ImportExcel_DanhSachNhanVien(Guid idDonVi)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT DANH SÁCH NHÂN VIÊN";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileDanhSachNhanVien(excelstream, idDonVi);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost] // todo
        public IHttpActionResult ImportDanhSachNhanVien_WithError(string RownError, Guid idDonVi)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorDanhSachNhanVien(excelstream, RownError, idDonVi);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin nhân viên
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinNhanVien(Guid ID_ChiNhanh)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = _Class_officeDocument.CheckFileMauThongTinNhanVien(excelstream);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinNhanVien(excelstream, ID_ChiNhanh);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinNhanVien_WithError(string RownError, Guid ID_ChiNhanh)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinNhanVien(excelstream, RownError, ID_ChiNhanh);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin hợp đồng
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinHopDong()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = _Class_officeDocument.CheckFileMauThongTinHopDong(excelstream);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinHopDong(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinHopDong_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinHopDong(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin bảo hiểm
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinBaoHiem()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = _Class_officeDocument.CheckFileMauThongTinBaoHiem(excelstream);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinBaoHiem(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinBaoHiem_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinBaoHiem(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin khen thưởng
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinKhenThuong()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN KHEN THƯỞNG - PHỤ CẤP";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinKhenThuong(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }

                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinKhenThuong_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinKhenThuong(excelstream, RownError);
                        }
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin khoản lương
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinKhoanLuong()
        {
            string result = "";
            try
            {
                List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN KHOẢN LƯƠNG - PHỤ CẤP";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinKhoanLuong(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinKhoanLuong_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinKhoanLuong(excelstream, RownError);
                        }
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin miễn giảm thuế
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinMienGiamThue()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN MIỄN GIẢM THUẾ";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinMienGiamThue(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinMienGiamThue_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinMienGiamThue(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin quy trình đào tạo
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinQuyTrinhDaoTao()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN QUY TRÌNH ĐÀO TẠO";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinQuyTrinhDaoTao(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinQuyTrinhDaoTao_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinQuyTrinhDaoTao(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin quá trình công tác
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinQuaTrinhCongTac()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN QUY TRÌNH ĐÀO TẠO";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinQuaTrinhCongTac(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinQuaTrinhCongTac_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinQuaTrinhCongTac(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin gia đình
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinGiaDinh()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN GIA ĐÌNH";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinGiaDinh(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinGiaDinh_WithError(string RownError)
        {
            string result = "";
            try
            {

                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinGiaDinh(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin chính trị
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinChinhTri()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN CHÍNH TRỊ";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinChinhTri(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinChinhTri_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinChinhTri(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        // thông tin sức khỏe
        [HttpPost]
        public IHttpActionResult ImportExcel_ThongTinSucKhoe()
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN SỨC KHỎE NHÂN VIÊN";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinSuKhoe(excelstream);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpPost]
        public IHttpActionResult ImportThongTinSucKhoe_WithError(string RownError)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinSucKhoe(excelstream, RownError);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        #endregion
        public string Check_Exist([FromBody] JObject data)
        {
            NS_NhanVien obj = data.ToObject<NS_NhanVien>();
            string err = string.Empty;
            int countObj = 0;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                // update:
                //if (obj.ID.ToString().IndexOf("0000") == -1)
                if (Guid.Empty != obj.ID)
                {

                    if (obj.MaNhanVien != null)
                    {

                        // check ma trung
                        countObj = _ClassNS_NhanVien.Gets(manv => manv.MaNhanVien == obj.MaNhanVien && manv.ID != obj.ID &&
                        (manv.TrangThai == 1 || manv.TrangThai == null)).Count();
                        if (countObj == 0)
                        {
                            // check SDT trung
                            if (obj.DienThoaiDiDong != null && obj.DienThoaiDiDong != "")
                            {
                                countObj = _ClassNS_NhanVien.Gets(manv => manv.DienThoaiDiDong == obj.DienThoaiDiDong && manv.ID != obj.ID && manv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Count();
                                if (countObj > 0)
                                {
                                    err = "Số điện thoại đã tồn tại";
                                }
                            }
                            // check SoCMT
                            if (obj.SoCMND != null && obj.SoCMND != "")
                            {
                                countObj = _ClassNS_NhanVien.Gets(manv => manv.SoCMND == obj.SoCMND && manv.ID != obj.ID && manv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Count();
                                if (countObj > 0)
                                {
                                    err = "Số chứng minh nhân dân đã tồn tại";
                                }
                            }
                            // check soBaoHiem
                            if (obj.SoBHXH != null && obj.SoBHXH.Trim() != "")
                            {
                                countObj = _ClassNS_NhanVien.Gets(manv => manv.SoBHXH == obj.SoBHXH && manv.ID != obj.ID).Count();
                                if (countObj > 0)
                                {
                                    err = "Số bảo hiểm đã tồn tại";
                                }
                            }
                        }
                        else
                        {
                            err = "Mã nhân viên đã tồn tại";
                        }

                    }
                    else
                    {
                        // check SDT trung
                        if (obj.DienThoaiDiDong != null && obj.DienThoaiDiDong != "")
                        {
                            countObj = _ClassNS_NhanVien.Gets(manv => manv.DienThoaiDiDong == obj.DienThoaiDiDong && manv.ID != obj.ID).Count();
                            if (countObj > 0)
                            {
                                err = "Số điện thoại đã tồn tại";
                            }
                        }
                        // check soCMT
                        if (obj.SoCMND != null && obj.SoCMND != "")
                        {
                            countObj = _ClassNS_NhanVien.Gets(manv => manv.SoCMND == obj.SoCMND && manv.ID != obj.ID && manv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Count();
                            if (countObj > 0)
                            {
                                err = "Số chứng minh nhân dân đã tồn tại";
                            }
                        }
                        // check soBaoHiem
                        if (obj.SoBHXH != null && obj.SoBHXH.Trim() != "")
                        {
                            countObj = _ClassNS_NhanVien.Gets(manv => manv.SoBHXH == obj.SoBHXH && manv.ID != obj.ID).Count();
                            if (countObj > 0)
                            {
                                err = "Số bảo hiểm đã tồn tại";
                            }
                        }
                    }
                }
                // insert
                else
                {
                    if (obj.MaNhanVien != null)
                    {
                        // check ma trung
                        countObj = _ClassNS_NhanVien.Gets(manv => manv.MaNhanVien == obj.MaNhanVien).Count();
                        if (countObj == 0)
                        {
                            //// check SDT trung
                            if (obj.DienThoaiDiDong != null && obj.DienThoaiDiDong.Trim() != "")
                            {
                                countObj = _ClassNS_NhanVien.Gets(manv => manv.DienThoaiDiDong == obj.DienThoaiDiDong).Count();
                                if (countObj > 0)
                                {
                                    err = "Số điện thoại đã tồn tại";
                                }
                            }
                            // check SoCMT
                            if (obj.SoCMND != null && obj.SoCMND.Trim() != "")
                            {
                                countObj = _ClassNS_NhanVien.Gets(manv => manv.SoCMND == obj.SoCMND && manv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Count();
                                if (countObj > 0)
                                {
                                    err = "Số chứng minh nhân dân đã tồn tại";
                                }
                            }
                            // check SoBaoHiem
                            if (obj.SoBHXH != null && obj.SoBHXH.Trim() != "")
                            {
                                countObj = _ClassNS_NhanVien.Gets(manv => manv.SoBHXH == obj.SoBHXH).Count();
                                if (countObj > 0)
                                {
                                    err = "Số bảo hiểm đã tồn tại";
                                }
                            }
                        }
                        else
                        {
                            err = "Mã nhân viên đã tồn tại";
                        }
                    }
                    else
                    {
                        // check SDT trung
                        if (obj.DienThoaiDiDong != null && obj.DienThoaiDiDong.Trim() != "")
                        {
                            countObj = _ClassNS_NhanVien.Gets(manv => manv.DienThoaiDiDong == obj.DienThoaiDiDong).Count();
                            if (countObj > 0)
                            {
                                err = "Số điện thoại đã tồn tại";
                            }
                        }
                        // check soCMT
                        if (obj.SoCMND != null && obj.SoCMND.Trim() != "")
                        {
                            countObj = _ClassNS_NhanVien.Gets(manv => manv.SoCMND == obj.SoCMND && manv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Count();
                            if (countObj > 0)
                            {
                                err = "Số chứng minh nhân dân đã tồn tại";
                            }
                        }
                        // check SoBaoHiem
                        if (obj.SoBHXH != null && obj.SoBHXH.Trim() != "")
                        {
                            countObj = _ClassNS_NhanVien.Gets(manv => manv.SoBHXH == obj.SoBHXH).Count();
                            if (countObj > 0)
                            {
                                err = "Số bảo hiểm đã tồn tại";
                            }
                        }
                    }
                }
                return err;
            }
        }

        #region delete
        // DELETE: api/NS_NhanVienAPI/5
        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult DeleteNS_NhanVien(Guid id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                string strDel = new ClassNS_NhanVien(db).Delete_NhanVien(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        [AcceptVerbs("GET")]
        public string getID_NhanVienAdmin()
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tb = from nd in db.HT_NguoiDung
                     where nd.ID.ToString().Contains("28FEF5A1-F0F2-4B94-A4AD-081B227F3B77")
                     select nd.ID_NhanVien;
            return tb.FirstOrDefault().Value.ToString();
        }
        #endregion
        public IHttpActionResult getListNhanVien(Guid? phongbanId, string maNhanVien, int trangthai, int pageSize, int pageNum)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                var lst_nv = new ClassNS_NhanVien(db).getListNhanViens_news(phongbanId, maNhanVien, trangthai);
                int Row = lst_nv.Count();
                //List<ListLHPages> lstPage = getAllPagenew<NS_NhanVien_new>(lst_nv, pageSize);
                int lstPages = getNumber_Page(Row, pageSize);
                List<NS_NhanVien_new> lst = lst_nv.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList().Select(nv => new NS_NhanVien_new
                {
                    ID = nv.ID,
                    TenNhanVien = nv.TenNhanVien,
                    MaNhanVien = nv.MaNhanVien,
                    DienThoaiDiDong = nv.DienThoaiDiDong,
                    GioiTinh = nv.GioiTinh,
                    SoBHXH = nv.SoBHXH,
                    SoCMND = nv.SoCMND,
                    TenNhanVien_KhongDau = nv.TenNhanVienKhongDau,
                    TenNhanVien_ChuCaiDau = nv.TenNhanVienChuCaiDau,
                    GhiChu = nv.GhiChu,
                    NgaySinh = nv.NgaySinh,
                    ThuongTru = nv.ThuongTru,
                    NguyenQuan = nv.NguyenQuan,
                    Email = nv.Email,
                    DaNghiViec = nv.DaNghiViec,
                    Image = nv.NS_NhanVien_Anh.Any() ? nv.NS_NhanVien_Anh.FirstOrDefault().URLAnh : string.Empty,
                    GhiChuThongTinChinhTri = nv.GhiChuThongTinChinhTri,
                    NgayNhapNgu = nv.NgayNhapNgu,
                    NgayVaoDangChinhThuc = nv.NgayVaoDangChinhThuc,
                    NgayVaoDoan = nv.NgayVaoDoan,
                    NgayXuatNgu = nv.NgayXuatNgu,
                    NoiSinhHoatDang = nv.NoiSinhHoatDang,
                    NoiVaoDoan = nv.NoiVaoDoan,
                    NgayRoiDang = nv.NgayRoiDang,
                    NgayVaoDang = nv.NgayVaoDang,
                    CMND = nv.SoCMND,
                    CMND_NgayCap = nv.NgayCap,
                    CMND_NoiCap = nv.NoiCap,
                    DanToc = nv.DanTocTonGiao,
                    TonGiao = nv.TonGiao,
                    HK_DiaChi = nv.DiaChiHKTT,
                    HK_QH = nv.DM_QuanHuyenHKTT != null ? nv.DM_QuanHuyenHKTT.TenQuanHuyen : string.Empty,
                    HK_TT = nv.DM_TinhThanhHKTT != null ? nv.DM_TinhThanhHKTT.TenTinhThanh : string.Empty,
                    HK_XP = nv.DM_XaPhuongHKTT != null ? nv.DM_XaPhuongHKTT.TenXaPhuong : string.Empty,
                    NgayVaolam = nv.NgayVaoLamViec,
                    NoiSinh = nv.NoiSinh,
                    PhongBan = nv.NS_PhongBan != null ? nv.NS_PhongBan.TenPhongBan : string.Empty,
                    QuocTich = nv.DM_QuocGia != null ? nv.DM_QuocGia.TenQuocGia : string.Empty,
                    TinhTrangHonNhan = commonEnumHellper.ListFamily.Any(o => o.Key.Equals(nv.TinhTrangHonNhan)) ? commonEnumHellper.ListFamily.FirstOrDefault(o => o.Key.Equals(nv.TinhTrangHonNhan)).Value : string.Empty,
                    TT_DiaChi = nv.DiaChiTT,
                    TT_QH = nv.DM_QuanHuyenTT != null ? nv.DM_QuanHuyenTT.TenQuanHuyen : string.Empty,
                    TT_TT = nv.DM_TinhThanhTT != null ? nv.DM_TinhThanhTT.TenTinhThanh : string.Empty,
                    TT_XP = nv.DM_XaPhuongTT != null ? nv.DM_XaPhuongTT.TenXaPhuong : string.Empty,
                }).ToList();


                JsonResultExampleTr<NS_NhanVien_new> json = new JsonResultExampleTr<NS_NhanVien_new>()
                {
                    Rowcount = Row,
                    LstData = lst,
                    numberPage = lstPages
                    //LstPageNumber = lstPage
                };
                return Json(json);
            }
        }
        [HttpGet]
        public void Export_NhanVienToExcel(Guid? phongbanId, string maNhanVien, int trangthai, string columnsHide)
        {
            string MNV_search = "%%";
            string MNVTV_search = "%%";
            if (maNhanVien != "" & maNhanVien != null & maNhanVien != "null")
            {
                MNV_search = "%" + CommonStatic.ConvertToUnSign(maNhanVien).ToLower() + "%";
                MNVTV_search = "%" + maNhanVien + "%";
            }
            string TT_search = "%%";
            if (trangthai == 1)
            {
                TT_search = "1";
            }
            if (trangthai == 2)
            {
                TT_search = "0";
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<NS_NhanVienPROC> lst = new List<NS_NhanVienPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("MaNhanVien", MNV_search));
            sql.Add(new SqlParameter("MaNhanVien_TV", MNVTV_search));
            sql.Add(new SqlParameter("TrangThai", TT_search));
            lst = db.Database.SqlQuery<NS_NhanVienPROC>("exec getList_DanhSachNhanVien @MaNhanVien, @MaNhanVien_TV, @TrangThai", sql.ToArray()).ToList();

            Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
            DataTable excel = _Class_officeDocument.ToDataTable<NS_NhanVienPROC>(lst);
            string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_ThongTinCoBanNhanVien.xlsx");
            string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachNhanVien.xlsx");
            fileSave = _Class_officeDocument.createFolder_Download(fileSave);
            _Class_officeDocument.listToOfficeExcel(fileTeamplate, fileSave, excel, 4, 28, 24, true, columnsHide);
            HttpResponse Response = HttpContext.Current.Response;
            _Class_officeDocument.downloadFile(fileSave);
        }

        //[System.Web.Http.AcceptVerbs("GET", "POST")]
        ////[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public async Task Cardpayment()
        //{
        //    string myid = "";
        //}

        [HttpPost]
        public IHttpActionResult getListNhanVienHRM(NhanVienFilter model)
        {
            try
            {
                //var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                //var service = new Model.Service.CommonService();
                //string url = "http://localhost:4414/api/DanhMuc/NS_NhanVienAPI/Cardpayment";
                //service.SendWebClientThread(url, null, "GET");
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var lst_nv = _NhanSuService.getListNhanVien_HRM(model);
                    int Row = lst_nv.Count();
                    int lstPages = getNumber_Page(Row, model.pageSize);
                    //List<ListLHPages> lstPage = getAllPagenew<NS_NhanVien_new>(lst_nv, model.pageSize);
                    List<NS_NhanVien_new> lst = lst_nv.OrderByDescending(o => o.NgayTao).Skip(model.pageSize * (model.pageNum - 1)).Take(model.pageSize).ToList().AsEnumerable().Select(nv => new NS_NhanVien_new
                    {
                        ID = nv.ID,
                        TenNhanVien = nv.TenNhanVien,
                        MaNhanVien = nv.MaNhanVien,
                        DienThoaiDiDong = nv.DienThoaiDiDong,
                        GioiTinh = nv.GioiTinh,
                        SoBHXH = nv.SoBHXH,
                        SoCMND = nv.SoCMND,
                        TenNhanVien_KhongDau = CommonStatic.ConvertToUnSign(nv.TenNhanVien).ToLower(),
                        TenNhanVien_ChuCaiDau = CommonStatic.GetCharsStart(nv.TenNhanVien).ToLower(),
                        GhiChu = nv.GhiChu,
                        NgaySinh = nv.NgaySinh,
                        ThuongTru = nv.ThuongTru,
                        NguyenQuan = nv.NguyenQuan,
                        Email = nv.Email,
                        DaNghiViec = nv.DaNghiViec,
                        Image = nv.NS_NhanVien_Anh.Any() ? nv.NS_NhanVien_Anh.FirstOrDefault().URLAnh : string.Empty,
                        GhiChuThongTinChinhTri = nv.GhiChuThongTinChinhTri,
                        NgayNhapNgu = nv.NgayNhapNgu,
                        NgayVaoDangChinhThuc = nv.NgayVaoDangChinhThuc,
                        NgayVaoDoan = nv.NgayVaoDoan,
                        NgayXuatNgu = nv.NgayXuatNgu,
                        NoiSinhHoatDang = nv.NoiSinhHoatDang,
                        NoiVaoDoan = nv.NoiVaoDoan,
                        NgayRoiDang = nv.NgayRoiDang,
                        NgayVaoDang = nv.NgayVaoDang,
                        CMND = nv.SoCMND,
                        CMND_NgayCap = nv.NgayCap,
                        CMND_NoiCap = nv.NoiCap,
                        DanToc = nv.DanTocTonGiao,
                        TonGiao = nv.TonGiao,
                        HK_DiaChi = nv.DiaChiHKTT,
                        HK_QH = nv.DM_QuanHuyenHKTT != null ? nv.DM_QuanHuyenHKTT.TenQuanHuyen : string.Empty,
                        HK_TT = nv.DM_TinhThanhHKTT != null ? nv.DM_TinhThanhHKTT.TenTinhThanh : string.Empty,
                        HK_XP = nv.DM_XaPhuongHKTT != null ? nv.DM_XaPhuongHKTT.TenXaPhuong : string.Empty,
                        NgayVaolam = nv.NgayVaoLamViec,
                        NoiSinh = nv.NoiSinh,
                        PhongBan = nv.NS_QuaTrinhCongTac.Any(o => o.LaDonViHienThoi) ?
                        (nv.NS_QuaTrinhCongTac.FirstOrDefault(o => o.LaDonViHienThoi).NS_PhongBan != null ? nv.NS_QuaTrinhCongTac.FirstOrDefault(o => o.LaDonViHienThoi).NS_PhongBan.TenPhongBan : string.Empty) : string.Empty,
                        QuocTich = nv.DM_QuocGia != null ? nv.DM_QuocGia.TenQuocGia : string.Empty,
                        TinhTrangHonNhan = commonEnumHellper.ListFamily.Any(o => o.Key.Equals(nv.TinhTrangHonNhan)) ? commonEnumHellper.ListFamily.FirstOrDefault(o => o.Key.Equals(nv.TinhTrangHonNhan)).Value : string.Empty,
                        TT_DiaChi = nv.DiaChiTT,
                        TT_QH = nv.DM_QuanHuyenTT != null ? nv.DM_QuanHuyenTT.TenQuanHuyen : string.Empty,
                        TT_TT = nv.DM_TinhThanhTT != null ? nv.DM_TinhThanhTT.TenTinhThanh : string.Empty,
                        TT_XP = nv.DM_XaPhuongTT != null ? nv.DM_XaPhuongTT.TenXaPhuong : string.Empty,
                        NgayTao = nv.NgayTao
                    }).ToList();

                    JsonResultExampleTr<NS_NhanVien_new> json = new JsonResultExampleTr<NS_NhanVien_new>()
                    {
                        Rowcount = Row,
                        LstData = lst,
                        numberPage = lstPages
                        //LstPageNumber = lstPage
                    };
                    return ActionTrueData(json);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }



        [HttpPost]
        public IHttpActionResult EditStaff(NhanVienModel model, Guid ID_DonVi, Guid ID_NhanVien, Guid? IdSaveAgain)
        {
            try
            {
                bool IsNew = true;
                if (model != null)
                {
                    NS_NhanVien objNhanVien = model.nhanvien;
                    if (objNhanVien.NgaySinh.HasValue && objNhanVien.NgaySinh.Value.Date > DateTime.Now.Date)
                    {
                        return ActionFalseNotData("Ngày sinh vượt quá ngày hiện tại");
                    }
                    if (string.IsNullOrWhiteSpace(objNhanVien.SoCMND))
                    {
                        objNhanVien.NgayCap = null;
                        objNhanVien.NoiCap = null;
                    }
                    if (!string.IsNullOrWhiteSpace(objNhanVien.DanTocTonGiao))
                    {
                        var dantoc = commonEnumHellper.ListDanToc.Any(o => o.Key.Equals(objNhanVien.DanTocTonGiao)) ? commonEnumHellper.ListDanToc.FirstOrDefault(o => o.Key.Equals(objNhanVien.DanTocTonGiao)).Value : string.Empty;
                        objNhanVien.DanTocTonGiao = dantoc;
                    }
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        string sMaNhanVien = string.Empty;
                        if (objNhanVien.ID == null || objNhanVien.ID == new Guid()) //Insert
                        {

                            objNhanVien.NgayTao = DateTime.Now;
                            if (string.IsNullOrWhiteSpace(objNhanVien.MaNhanVien))
                            {
                                //objNhanVien.MaNhanVien = GetMaNhanVien();
                                SqlParameter sql = new SqlParameter("MaNhanVien", "NV00001");
                                objNhanVien.MaNhanVien = db.Database.SqlQuery<string>("exec get_MaNhanVien @MaNhanVien", sql).FirstOrDefault().Trim();
                            }
                            else
                            {
                                if (db.NS_NhanVien.Any(o => o.MaNhanVien.Equals(objNhanVien.MaNhanVien)))
                                {
                                    return ActionFalseNotData("Mã nhân viên bạn nhập đã bị trùng");
                                }
                            }
                            objNhanVien.SoCMND = objNhanVien.SoCMND != null ? objNhanVien.SoCMND.Trim().ToUpper() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(objNhanVien.SoCMND))
                            {
                                if (db.NS_NhanVien.Any(o => o.SoCMND.Equals(objNhanVien.SoCMND.Trim()) && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa))
                                    return ActionFalseNotData("Chứng minh thư nhân dân đã bị trùng");
                            }
                            objNhanVien.ID = Guid.NewGuid();
                            objNhanVien.TenNhanVienKhongDau = CommonStatic.RemoveSign4VietnameseString(objNhanVien.TenNhanVien);
                            objNhanVien.TenNhanVienChuCaiDau = CommonStatic.convertchartstart(objNhanVien.TenNhanVien);
                            db.NS_NhanVien.Add(objNhanVien);
                            if (IdSaveAgain != null && IdSaveAgain != new Guid())// Lưu và thêm mới 
                            {
                                var listImage = db.NS_NhanVien_Anh.Where(o => o.ID_NhanVien == IdSaveAgain).ToList().Select(o => new NS_NhanVien_Anh
                                {
                                    ID = Guid.NewGuid(),
                                    ID_NhanVien = objNhanVien.ID,
                                    SoThuTu = o.SoThuTu,
                                    URLAnh = o.URLAnh
                                }).ToList();
                                db.NS_NhanVien_Anh.AddRange(listImage);
                            }
                        }
                        else // Update
                        {
                            IsNew = false;
                            if (!string.IsNullOrWhiteSpace(objNhanVien.MaNhanVien))
                            {
                                if (db.NS_NhanVien.Any(o => o.ID != objNhanVien.ID && o.MaNhanVien.Equals(objNhanVien.MaNhanVien) && (o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa || o.TrangThai == null)))
                                {
                                    return ActionFalseNotData("Mã nhân viên bạn nhập đã bị trùng");
                                }
                            }
                            var modelNv = db.NS_NhanVien.FirstOrDefault(o => o.ID == objNhanVien.ID);
                            if (modelNv == null)
                            {
                                return ActionFalseNotData("Nhân viên đã bị xóa hoặc không tồn tại");
                            }
                            if (!string.IsNullOrWhiteSpace(objNhanVien.SoCMND) && db.NS_NhanVien.Any(o => o.SoCMND.Equals(objNhanVien.SoCMND.ToUpper().Trim()) && o.ID != modelNv.ID && o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa))
                            {
                                return ActionFalseNotData("Chứng minh thư nhân dân đã bị trùng");
                            }
                            if (!string.IsNullOrWhiteSpace(objNhanVien.MaNhanVien))
                            {
                                modelNv.MaNhanVien = objNhanVien.MaNhanVien.ToUpper();
                            }
                            modelNv.NguoiSua = objNhanVien.NguoiTao;
                            modelNv.NgaySua = DateTime.Now;
                            modelNv.TenNhanVien = objNhanVien.TenNhanVien;
                            modelNv.TenNhanVienKhongDau = CommonStatic.RemoveSign4VietnameseString(objNhanVien.TenNhanVien);
                            modelNv.TenNhanVienChuCaiDau = CommonStatic.convertchartstart(objNhanVien.TenNhanVien);
                            modelNv.GioiTinh = objNhanVien.GioiTinh;
                            modelNv.DienThoaiDiDong = objNhanVien.DienThoaiDiDong;
                            modelNv.ID_QuocGia = objNhanVien.ID_QuocGia;
                            modelNv.DienThoaiNhaRieng = objNhanVien.DienThoaiNhaRieng;
                            modelNv.Email = objNhanVien.Email;
                            modelNv.NoiSinh = objNhanVien.NoiSinh;
                            modelNv.SoCMND = objNhanVien.SoCMND != null ? objNhanVien.SoCMND.Trim().ToUpper() : string.Empty;
                            modelNv.NoiCap = objNhanVien.NoiCap;
                            modelNv.DanTocTonGiao = objNhanVien.DanTocTonGiao;
                            modelNv.GhiChu = objNhanVien.GhiChu;
                            modelNv.ID_TinhThanhHKTT = objNhanVien.ID_TinhThanhHKTT;
                            modelNv.ID_TinhThanhTT = objNhanVien.ID_TinhThanhTT;
                            modelNv.ID_QuanHuyenHKTT = objNhanVien.ID_QuanHuyenHKTT;
                            modelNv.ID_XaPhuongHKTT = objNhanVien.ID_XaPhuongHKTT;
                            modelNv.ID_QuanHuyenTT = objNhanVien.ID_QuanHuyenTT;
                            modelNv.ID_XaPhuongTT = objNhanVien.ID_XaPhuongTT;
                            modelNv.NgayVaoLamViec = objNhanVien.NgayVaoLamViec;
                            modelNv.NgaySinh = objNhanVien.NgaySinh;
                            modelNv.NgayCap = objNhanVien.NgayCap;
                            modelNv.NguyenQuan = objNhanVien.NguyenQuan;
                            modelNv.ThuongTru = objNhanVien.ThuongTru;
                            modelNv.DaNghiViec = objNhanVien.DaNghiViec;
                            modelNv.ID_NSPhongBan = objNhanVien.ID_NSPhongBan;
                            modelNv.TonGiao = objNhanVien.TonGiao;
                            modelNv.TinhTrangHonNhan = objNhanVien.TinhTrangHonNhan;
                            modelNv.DiaChiTT = objNhanVien.DiaChiTT;
                            modelNv.DiaChiHKTT = objNhanVien.DiaChiHKTT;
                            var list_delete = db.NS_QuaTrinhCongTac.Where(o => o.ID_NhanVien == objNhanVien.ID).ToList();
                            db.NS_QuaTrinhCongTac.RemoveRange(list_delete);

                            // update ht_nguoidung (nghiviec: tamngung nguoidung/nguoclai)
                            db.HT_NguoiDung.Where(x => x.ID_NhanVien == objNhanVien.ID).ToList().ForEach(x => x.DangHoatDong = !objNhanVien.DaNghiViec);
                        }
                        string chitiet_km = "Họ tên: " + objNhanVien.TenNhanVien;
                        string noidung_km = ". Họ tên: " + objNhanVien.TenNhanVien;
                        if (objNhanVien.NgaySinh != null)
                        {
                            noidung_km = noidung_km + ". Ngày sinh: " + objNhanVien.NgaySinh.Value.ToString("dd/MM/yyyy");
                            chitiet_km = chitiet_km + "<br>Ngày sinh: " + objNhanVien.NgaySinh.Value.ToString("dd/MM/yyyy");
                        }
                        noidung_km = noidung_km + ". Giới tính: " + (objNhanVien.GioiTinh == true ? "Nam" : "Nữ") + ". Trạng thái: " + (objNhanVien.DaNghiViec == false ? "Đang làm việc" : "Đã nghỉ việc");
                        chitiet_km = chitiet_km + "<br>Giới tính: " + (objNhanVien.GioiTinh == true ? "Nam" : "Nữ") + "<br>Trạng thái: " + (objNhanVien.DaNghiViec == false ? "Đang làm việc" : "Đã nghỉ việc");
                        if (objNhanVien.SoCMND != "" & objNhanVien.SoCMND != null & objNhanVien.SoCMND != "null")
                        {
                            noidung_km = noidung_km + ". Số CMND: " + objNhanVien.SoCMND;
                            chitiet_km = chitiet_km + "<br>Số CMND: " + objNhanVien.SoCMND;
                        }
                        if (objNhanVien.ThuongTru != "" & objNhanVien.ThuongTru != null & objNhanVien.ThuongTru != "null")
                        {
                            noidung_km = noidung_km + ". Địa chỉ thường trú: " + objNhanVien.DiaChiTT;
                            chitiet_km = chitiet_km + "<br>Địa chỉ thường trú: " + objNhanVien.DiaChiTT;
                        }
                        if (objNhanVien.DienThoaiDiDong != "" & objNhanVien.DienThoaiDiDong != null & objNhanVien.DienThoaiDiDong != "null")
                        {
                            noidung_km = noidung_km + ". Điện thoại: " + objNhanVien.DienThoaiDiDong;
                            chitiet_km = chitiet_km + "<br>Điện thoại: " + objNhanVien.DienThoaiDiDong;
                        }
                        if (objNhanVien.Email != "" & objNhanVien.Email != null & objNhanVien.Email != "null")
                        {
                            noidung_km = noidung_km + ". Email: " + objNhanVien.Email;
                            chitiet_km = chitiet_km + "<br>Email: " + objNhanVien.Email;
                        }
                        if (objNhanVien.GhiChu != "" & objNhanVien.GhiChu != null & objNhanVien.GhiChu != "null")
                        {
                            noidung_km = noidung_km + ". Ghi chú: " + objNhanVien.GhiChu;
                            chitiet_km = chitiet_km + "<br>Ghi chú: " + objNhanVien.GhiChu;
                        }
                        var phongbanmacdinh = db.NS_PhongBan.Where(o => o.ID_DonVi == null).Select(o => o.ID).FirstOrDefault();
                        var donvimacdinh = contant.GetUserCookies().ID_DonVi ?? new Guid();
                        if (model.QuaTrinhCongTac != null && model.QuaTrinhCongTac.Any())
                        {
                            var listQTCT = model.QuaTrinhCongTac.Select(o => o.ID_ChiNhanh).Distinct().ToList();
                            var listdv = db.DM_DonVi.Where(x => listQTCT.Contains(x.ID)).Select(o => o.TenDonVi).ToArray();
                            noidung_km += ". Chi nhánh làm việc: " + string.Join(",", listdv);
                            chitiet_km += "<br>Chi nhánh làm việc: " + string.Join(",", listdv);

                            var listCongTacNew = model.QuaTrinhCongTac.Select(o => new NS_QuaTrinhCongTac
                            {
                                ID = Guid.NewGuid(),
                                ID_NhanVien = objNhanVien.ID,
                                ID_DonVi = o.ID_ChiNhanh ?? donvimacdinh,
                                LaDonViHienThoi = o.LaMacDinh,
                                ID_PhongBan = o.ID_PhongBan ?? phongbanmacdinh,
                                NgayApDung = DateTime.Now
                            }).ToList();
                            db.NS_QuaTrinhCongTac.AddRange(listCongTacNew);
                        }
                        else
                        {
                            db.NS_QuaTrinhCongTac.Add(new NS_QuaTrinhCongTac
                            {
                                ID = Guid.NewGuid(),
                                ID_NhanVien = objNhanVien.ID,
                                ID_DonVi = donvimacdinh,
                                LaDonViHienThoi = true,
                                ID_PhongBan = phongbanmacdinh,
                                NgayApDung = DateTime.Now
                            });
                        }
                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = ID_NhanVien,
                            ID_DonVi = ID_DonVi,
                            ChucNang = "Nhân viên",
                            ThoiGian = DateTime.Now,
                            NoiDung = IsNew ? "Thêm mới nhân viên: " + objNhanVien.TenNhanVien : "Cập nhật thông tin nhân viên: " + objNhanVien.TenNhanVien,
                            NoiDungChiTiet = (IsNew ? "Thêm mới nhân viên" : "Cập nhật thông tin nhân viên") + ": <a style= \"cursor: pointer\" onclick = \"loadNhanVienbyMaKM('" + objNhanVien.MaNhanVien + "')\" >" + objNhanVien.MaNhanVien + "</a> <br>" + chitiet_km,
                            LoaiNhatKy = IsNew ? 1 : 2
                        };
                        db.HT_NhatKySuDung.Add(hT_NhatKySuDung);
                        db.SaveChanges();
                        return ActionTrueData(objNhanVien.ID);
                    }
                }
                return ActionFalseNotData("Không lấy được thông tin nhân viên.");
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult ExportExcelNhanVien(NhanVienFilterExport model)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    if (model.TrangThai == 0 || model.TrangThai == null)
                    {
                        model.TrangThai = 2;
                    }
                    else if (model.TrangThai == 2)
                    {
                        model.TrangThai = 0;
                    }
                    var listPhongban = new List<Guid>();
                    if (model.PhongBanId != null)
                    {
                        var phongban = db.NS_PhongBan.FirstOrDefault(o => o.ID == model.PhongBanId);
                        if (phongban != null && phongban.ID_PhongBanCha != null)
                        {
                            listPhongban = db.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                        }
                        else if (phongban != null)
                        {
                            listPhongban = db.NS_PhongBan.Where(o => o.ID_PhongBanCha == phongban.ID || o.ID == phongban.ID).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                            var listPhongbannew = db.NS_PhongBan.Where(o => listPhongban.Contains(o.ID_PhongBanCha ?? new Guid())).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).Select(o => o.ID).ToList();
                            listPhongban.AddRange(listPhongbannew);
                            listPhongban = listPhongban.Distinct().ToList();
                        }
                    }
                    var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                    var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    bool IsfilterTime = CommonStatic.CheckTimeFilter(model.TypeTime, model.TuNgay, model.DenNgay, ref startday, ref Endday);

                    int? typechinhtri = -1;
                    if (model.ChinhTri == null || model.ChinhTri.Count == 3)
                    {
                        typechinhtri = -1;
                    }
                    else if (model.ChinhTri.Count == 1)
                    {
                        typechinhtri = model.ChinhTri.First();

                    }
                    else if (model.ChinhTri.Count == 2)
                    {
                        if (model.ChinhTri.All(o => o == (int)commonEnumHellper.TypeTTChinhTri.ketnapdoan || o == (int)commonEnumHellper.TypeTTChinhTri.ketnapdang))
                        {
                            typechinhtri = 3;
                        }
                        else if (model.ChinhTri.All(o => o == (int)commonEnumHellper.TypeTTChinhTri.ketnapdoan || o == (int)commonEnumHellper.TypeTTChinhTri.danhapngu))
                        {
                            typechinhtri = 4;
                        }
                        else if (model.ChinhTri.All(o => o == (int)commonEnumHellper.TypeTTChinhTri.danhapngu || o == (int)commonEnumHellper.TypeTTChinhTri.ketnapdang))
                        {
                            typechinhtri = 5;
                        }
                    }

                    paramlist.Add(new SqlParameter("donviID", model.DonViId != null ? model.DonViId.ToString() : ""));
                    paramlist.Add(new SqlParameter("phongban", string.Join(",", listPhongban)));
                    paramlist.Add(new SqlParameter("dantoc", string.Join(",", model.DanToc ?? new List<string>())));
                    paramlist.Add(new SqlParameter("HK_TT", model.HK_TT != null ? model.HK_TT.ToString() : ""));
                    paramlist.Add(new SqlParameter("HK_QH", model.HK_QH != null ? model.HK_QH.ToString() : ""));
                    paramlist.Add(new SqlParameter("HK_XP", model.HK_XP != null ? model.HK_XP.ToString() : ""));
                    paramlist.Add(new SqlParameter("TT_TT", model.TT_TT != null ? model.TT_TT.ToString() : ""));
                    paramlist.Add(new SqlParameter("TT_QH", model.TT_QH != null ? model.TT_QH.ToString() : ""));
                    paramlist.Add(new SqlParameter("TT_XP", model.TT_XP != null ? model.TT_XP.ToString() : ""));
                    paramlist.Add(new SqlParameter("GioiTinh", model.GioiTinh == null ? 2 : (model.GioiTinh == true ? 1 : 0)));
                    paramlist.Add(new SqlParameter("TrangThai", model.TrangThai));
                    paramlist.Add(new SqlParameter("ChinhTri", typechinhtri));
                    paramlist.Add(new SqlParameter("Start", IsfilterTime ? startday.ToString("dd/MM/yyyy") : ""));
                    paramlist.Add(new SqlParameter("End", IsfilterTime ? Endday.AddDays(1).ToString("dd/MM/yyyy") : ""));
                    paramlist.Add(new SqlParameter("LoaiHopDong", string.Join(",", model.LoaiHopDong ?? new List<int>())));
                    paramlist.Add(new SqlParameter("ListBaoHiem", string.Join(",", model.BaoHiem ?? new List<int>())));
                    paramlist.Add(new SqlParameter("text", model.Text ?? ""));
                    var lst = db.Database.SqlQuery<ExportNhanVien>("exec SelectDanhSachNhanVien @donviID,@phongban,@dantoc,@HK_TT,@HK_QH,@HK_XP,@TT_TT,@TT_QH,@TT_XP,@GioiTinh,@TrangThai,@ChinhTri,@Start,@End,@LoaiHopDong,@ListBaoHiem,@text", paramlist.ToArray()).ToList();
                    Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                    DataTable excel = _Class_officeDocument.ToDataTable<ExportNhanVien>(lst);
                    excel.Columns.Remove("TinhTrangHonNhan");
                    excel.Columns.Remove("DaNghiViec");
                    excel.Columns.Remove("GioiTinh");
                    string fileTeamplate = System.Web.HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/Teamplate_DanhSachNhanVien.xlsx");
                    string fileSave = System.Web.HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoCao/Teamplate_DanhSachNhanVien.xlsx");
                    fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                    _Class_officeDocument.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 5, 29, 24, true, "", DateTime.Now.ToString("dd/MM/yyyy"), db.DM_DonVi.Where(o => o.ID == model.DonViId).Select(o => o.TenDonVi).FirstOrDefault());
                    System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                    return ActionTrueData(fileSave);
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }
        [HttpGet]
        public void DownloadExecl(string fileSave)
        {
            using (var db = SystemDBContext.GetDBContext())
            {

                new Class_officeDocument(db).downloadFile(fileSave);
            }
        }

        [HttpPost]
        public IHttpActionResult UploadImageStaff(Guid nhanvienId)
        {
            try
            {
                var path = "";
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];
                    var filenameImage = DateTime.Now.ToString("YYYY-MM-DD") + "_" + Guid.NewGuid().ToString() + ".jpg";
                    var subdomain = CookieStore.GetCookieAes("SubDomain");
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/FolderImage/" + subdomain + "/" + fliepath.nhanvien.ToString())))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/FolderImage/" + subdomain + "/" + fliepath.nhanvien.ToString()));
                    }

                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/FolderImage/" + subdomain + "/" + fliepath.nhanvien.ToString()), filenameImage);

                    file.SaveAs(path);
                    using (var db = SystemDBContext.GetDBContext())
                    {
                        var list_delete = db.NS_NhanVien_Anh.Where(o => o.ID_NhanVien == nhanvienId).ToList();
                        db.NS_NhanVien_Anh.RemoveRange(list_delete);
                        var model = new NS_NhanVien_Anh
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = nhanvienId,
                            SoThuTu = 1,
                            URLAnh = "/FolderImage/" + subdomain + "/" + fliepath.nhanvien.ToString() + "/" + filenameImage
                        };
                        db.NS_NhanVien_Anh.Add(model);
                        db.SaveChanges();
                    }
                }
                return ActionTrueNotData(string.Empty);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult UpdateAnhNhanVien([FromBody] List<string> files, Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    //ClassDM_DoiTuong_Anh classDoiTuongAnh = new ClassDM_DoiTuong_Anh(db);
                    //var count = classDoiTuongAnh.Gets(p => p.ID_DoiTuong == id).Count();
                    List<NS_NhanVien_Anh> lstRemove = db.NS_NhanVien_Anh.Where(p => p.ID_NhanVien == id).ToList();
                    if (lstRemove.Count > 0)
                    {
                        db.NS_NhanVien_Anh.RemoveRange(lstRemove);
                    }

                    List<NS_NhanVien_Anh> lst = new List<NS_NhanVien_Anh>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        //Add db table Anh
                        NS_NhanVien_Anh objAnh = new NS_NhanVien_Anh();
                        objAnh.ID = Guid.NewGuid();
                        objAnh.ID_NhanVien = id;
                        objAnh.SoThuTu = 1 + i;
                        objAnh.URLAnh = files[i];
                        lst.Add(objAnh);
                    }
                    db.NS_NhanVien_Anh.AddRange(lst);
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return Ok("");
        }

        public IHttpActionResult GetDetailStaff(Guid id)
        {
            try
            {
                using (var db = SystemDBContext.GetDBContext())
                {
                    var model = db.NS_NhanVien.Where(o => o.ID == id).ToList().Select(o => new
                    {
                        o.ID,
                        o.MaNhanVien,
                        o.GioiTinh,
                        o.DienThoaiDiDong,
                        o.DienThoaiNhaRieng,
                        o.Email,
                        o.NoiSinh,
                        o.SoCMND,
                        o.NoiCap,
                        o.NgayCap,
                        DanTocTonGiao = commonEnumHellper.ListDanToc.Any(c => c.Value.Equals(o.DanTocTonGiao)) ? commonEnumHellper.ListDanToc.FirstOrDefault(c => c.Value.Equals(o.DanTocTonGiao)).Key : o.DanTocTonGiao,
                        o.GhiChu,
                        o.ID_TinhThanhHKTT,
                        o.ID_QuanHuyenHKTT,
                        o.ID_XaPhuongHKTT,
                        o.ID_QuanHuyenTT,
                        o.ID_TinhThanhTT,
                        o.ID_XaPhuongTT,
                        o.NguyenQuan,
                        o.ThuongTru,
                        o.DaNghiViec,
                        o.NgayVaoLamViec,
                        o.NgaySinh,
                        o.TenNhanVien,
                        o.ID_NSPhongBan,
                        o.TonGiao,
                        o.TinhTrangHonNhan,
                        Image = o.NS_NhanVien_Anh.Any() ? o.NS_NhanVien_Anh.FirstOrDefault().URLAnh : "",
                        TenPhongBan = o.NS_PhongBan != null ? o.NS_PhongBan.TenPhongBan : string.Empty
                    }).FirstOrDefault();
                    if (model == null)
                    {
                        return ActionFalseNotData("Không tồn tại nhân viên hoặc nhân viên đã bị xóa khỏi hệ thống");
                    }
                    var listChiNhanhPhongBan = db.NS_QuaTrinhCongTac.Where(o => o.ID_NhanVien == model.ID).OrderByDescending(o => o.LaDonViHienThoi).AsEnumerable().Select(o => new QuaTrinhCongTacModel
                    {
                        ID_ChiNhanh = o.ID_DonVi,
                        ID_PhongBan = o.ID_PhongBan,
                        Text_ChiNhanh = o.DM_DonVi != null ? o.DM_DonVi.TenDonVi : string.Empty,
                        LaMacDinh = o.LaDonViHienThoi,
                        Text_PhongBan = o.NS_PhongBan != null ? o.NS_PhongBan.TenPhongBan : string.Empty,
                        listPhongBan = GetPhongBanByChiNhanh(o.ID_DonVi)
                    }).ToList();
                    return ActionTrueData(new { model, listChiNhanhPhongBan });
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult SaveThongTinChinhtri(NS_NhanVien_new model)
        {
            try
            {

                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.NgayVaoDangChinhThuc.HasValue && model.NgayVaoDang.HasValue && model.NgayVaoDang.Value.Date > model.NgayVaoDangChinhThuc.Value.Date)
                {
                    return ActionFalseNotData("Ngày vào đảng không thể lớn hơn ngày vào đảng chính thức");
                }
                if (model.NgayVaoDangChinhThuc.HasValue && model.NgayRoiDang.HasValue && model.NgayRoiDang.Value.Date < model.NgayVaoDangChinhThuc.Value.Date)
                {
                    return ActionFalseNotData("Ngày rời khỏi đảng không thể nhỏ hơn ngày vào đảng chính thức");
                }
                if (model.NgayVaoDang.HasValue && model.NgayRoiDang.HasValue && model.NgayRoiDang.Value.Date < model.NgayVaoDang.Value.Date)
                {
                    return ActionFalseNotData("Ngày rời khỏi đảng không thể nhỏ hơn ngày vào đảng");
                }
                if (model.NgayXuatNgu.HasValue && model.NgayNhapNgu.HasValue && model.NgayXuatNgu.Value.Date < model.NgayNhapNgu.Value.Date)
                {
                    return ActionFalseNotData("Ngày nhập ngũ không thể lớn hơn ngày nhập ngũ");
                }
                var UserLogin = contant.GetUserCookies();
                if (UserLogin.ID_DonVi == null)
                {
                    return ActionFalseNotData("Đã xảy ra lỗi người dùng truy cập hệ thống");
                }
                using (var db = SystemDBContext.GetDBContext())
                {
                    var result = new ClassNS_NhanVien(db).UpdatettChinhTri(model, UserLogin);

                    if (result.ErrorCode)
                        return ActionTrueNotData("Cập nhật thành công.");
                    else
                        return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveThongTinGiaDinh(NS_NhanVien_GiaDinh model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.NgaySinh == 0 || (model.NgaySinh.ToString().Length != 4
                    && model.NgaySinh.ToString().Length != 6 && model.NgaySinh.ToString().Length != 8))
                {
                    return ActionFalseNotData("Ngày sinh không đúng định dạng");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (model.ID == new Guid())// insert
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        _ClassNS_NhanVien.InsertNhanVienGiaDinh(model, history);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNhanVienGiaDinh(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveQuyTrinhDaoTao(NS_NhanVien_DaoTao model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.TuNgay != null && model.DenNgay != null && model.TuNgay >= model.DenNgay)
                {
                    return ActionFalseNotData("Từ ngày Không lớn hơn đến ngày");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (model.ID == new Guid())// insert
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        _ClassNS_NhanVien.InsertNvTTDaoTao(model, history);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvTTDaoTao(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveQuaTrinhCongTac(NS_NhanVien_CongTac model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.TuNgay != null && model.DenNgay != null && model.TuNgay >= model.DenNgay)
                {
                    return ActionFalseNotData("Từ ngày không lớn hơn đến ngày");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (model.ID == new Guid())// insert
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        _ClassNS_NhanVien.InsertNvQTCongTac(model, history);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvQTCongTac(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveTinhTrangSucKhoe(NS_NhanVien_SucKhoe model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.NgayKham == null)
                {
                    return ActionFalseNotData("Ngày khám không được để trống");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (model.ID == new Guid())// insert
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        _ClassNS_NhanVien.InsertNvTTSucKhoe(model, history);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvTTSuwcKhoe(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveHopDong(NS_HopDong model, bool Isnew = false)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.NgayKy == null)
                {
                    return ActionFalseNotData("Ngày ký không được để trống");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (Isnew)
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        var result = _ClassNS_NhanVien.InsertNvHopDong(model, history);
                        if (result.ErrorCode)
                        {
                            return InsertSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvHOpDong(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }


            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveBaoHiem(NS_BaoHiem model, bool Isnew = false)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.NgayCap == null)
                {
                    return ActionFalseNotData("Ngày cấp không được để trống");
                }
                if (model.NgayHetHan == null)
                {
                    return ActionFalseNotData("Ngày hết hạn không được để trống");
                }
                if (model.NgayHetHan.Value.Date <= model.NgayCap.Date)
                {
                    return ActionFalseNotData("Ngày hết hạn không được nhỏ hơn ngày cấp");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (Isnew)
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        _ClassNS_NhanVien.InsertNvBaoHiem(model, history);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvBaoHiem(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveKhenthuong(NS_KhenThuong model, bool Isnew = false)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.NgayBanHang == null)
                {
                    return ActionFalseNotData("Ngày ban hành không được để trống");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (Isnew)
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        _ClassNS_NhanVien.InsertNvKhenThuong(model, history);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvKhenThuong(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }


            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }


        [HttpPost]
        public IHttpActionResult SaveMienGiamthue(NS_MienGiamThue model, bool Isnew = false)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.NgayKetThuc != null && model.NgayKetThuc.Value.Date < model.NgayApDung.Date)
                {
                    return ActionFalseNotData("Ngày kết thúc không được nhỏ hơn ngày áp dụng");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (Isnew)
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        _ClassNS_NhanVien.InsertNvMienGiamThue(model, history);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvMienGiamThue(model, history);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveLuongPhuCap([FromBody] JObject data)
        {
            try
            {
                Boolean Isnew = data["IsNew"].ToObject<Boolean>();
                NS_Luong_PhuCap model = data["luongPhuCap"].ToObject<NS_Luong_PhuCap>();
                List<NS_ThietLapLuongChiTiet> lstDetail = new List<NS_ThietLapLuongChiTiet>();
                if (data["thietLapChiTiet"] != null && data["thietLapChiTiet"].Count() > 0)
                {
                    lstDetail = data["thietLapChiTiet"].ToObject<List<NS_ThietLapLuongChiTiet>>();
                }
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.LoaiLuong > 4 && model.ID_LoaiLuong == null)
                {
                    return ActionFalseNotData("Vui lòng chọn loại lương");
                }
                if (model.NgayKetThuc != null && model.NgayKetThuc.Value.Date < model.NgayApDung.Date)
                {
                    return ActionFalseNotData("Ngày kết thúc không được nhỏ hơn ngày áp dụng");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Thiết lập lương - phụ cấp";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    var exist = _ClassNS_NhanVien.CheckExist_ThietLapLuong(model);
                    if (exist)
                    {
                        return ActionFalseNotData("Đã thiết lập lương, phụ cấp trong khoảng thời gian này");
                    }

                    var txtNhanVien = string.Concat("thiết lập lương cho nhân viên ", db.NS_NhanVien.Find(model.ID_NhanVien).MaNhanVien, " - ", db.NS_NhanVien.Find(model.ID_NhanVien).TenNhanVien);
                    if (Isnew)
                    {
                        var result = _ClassNS_NhanVien.InsertNvLuongPhuCap(model, lstDetail);
                        if (result.ErrorCode == false)
                        {
                            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                            history.NoiDung = "Thêm mới " + txtNhanVien;
                            history.NoiDungChiTiet = string.Concat("Thêm mới ", txtNhanVien, result.Data);
                            db.HT_NhatKySuDung.Add(history);
                            db.SaveChanges();
                            return InsertSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvLuongPhuCap(model, lstDetail);
                        if (result.ErrorCode == false)
                        {
                            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;
                            history.NoiDung = "Cập nhật " + txtNhanVien;
                            history.NoiDungChiTiet = string.Concat("Cập nhật ", txtNhanVien, result.Data);
                            db.HT_NhatKySuDung.Add(history);
                            db.SaveChanges();
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveLoaiLuong(NS_LoaiLuong model, bool Isnew = false)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (string.IsNullOrWhiteSpace(model.TenLoaiLuong))
                {
                    return ActionFalseNotData("Vui lòng nhệp tên loại lương");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    if (Isnew)
                    {
                        model.ID = Guid.NewGuid();
                        model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                        var result = _ClassNS_NhanVien.InsertNvLoaiLuong(model, history);
                        if (result.ErrorCode)
                        {
                            return ActionTrueData(model);
                        }
                        return ActionFalseNotData(result.Data);
                    }
                    else
                    {
                        var result = _ClassNS_NhanVien.UpdateNvLoaiLuong(model, history);
                        if (result.ErrorCode)
                        {
                            return ActionTrueData(model);
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaoChepThietLapLuong(SaoChep_ThietLapLuong lstParam)
        {
            try
            {
                using (var db = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(db);
                    _NhanSuService.SaoChepThietLapLuong(lstParam);
                    return ActionTrueNotData("Sao chép thành công");
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpGet]
        public IHttpActionResult deleteTTSucKhoeNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvTTSucKhoe(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }
        [HttpGet]
        public IHttpActionResult deleteQTCongTacNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvQTCongTac(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpGet]
        public IHttpActionResult deleteQTDaoTaoNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvQTDaoTao(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }
        [HttpGet]
        public IHttpActionResult deleteGiaDinhNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvGiaDinh(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpGet]
        public IHttpActionResult deleteTTHopDongNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvHopDong(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }
        [HttpGet]
        public IHttpActionResult deleteBaoHiemNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvBaoHiem(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpGet]
        public IHttpActionResult deleteKhenThuongNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvKhenThuong(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }
        [HttpGet]
        public IHttpActionResult deleteMienGiamthueNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvMienGiamThue(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }
        [HttpGet]
        public IHttpActionResult deleteLuongPhuCapNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Nhân viên";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvLuongPhuCap(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpGet]
        public IHttpActionResult deleteLoaiLuongNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Loại lương";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvLoaiLuong(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpGet]
        public IHttpActionResult deletePhongBanNv(Guid id)
        {
            try
            {
                if (id == new Guid())
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Phòng ban";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    _ClassNS_NhanVien.DeleteNvPhongBan(id, history);
                }
                return DeleteSuccess();

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveNsPhongBan(NS_PhongBan model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần cập nhật");
                }
                if (model.TenPhongBan == null)
                {
                    return ActionFalseNotData("Tên phòng ban không được để trống");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Phòng ban";
                model.ID = Guid.NewGuid();
                model.TrangThai = (int)commonEnumHellper.TypeIsDelete.hoatdong;
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    var result = _ClassNS_NhanVien.InsertNsPhongban(model, history);
                    if (result.ErrorCode)
                    {
                        return InsertSuccess();
                    }
                    return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        [HttpPost]
        public IHttpActionResult EditNsPhongBan(NS_PhongBan model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần cập nhật");
                }
                if (model.ID_PhongBanCha != null && model.ID == model.ID_PhongBanCha)
                {
                    return ActionFalseNotData("Không thể chọn phòng ban cha là phong ban hiện tại");
                }
                if (model.TenPhongBan == null)
                {
                    return ActionFalseNotData("Tên phòng ban không được để trống");
                }
                var history = GetHistory();
                if (history == null)
                {
                    return ErrorNotFoundUser();
                }
                history.ChucNang = "Phòng ban";
                using (var db = SystemDBContext.GetDBContext())
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    var result = _ClassNS_NhanVien.UpdateNsPhongban(model, history);
                    if (result.ErrorCode)
                    {
                        return UpdateSuccess();
                    }
                    return ActionFalseNotData(result.Data);
                }

            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        public IHttpActionResult GetTTGiaDinhNv(Guid nhanvienId)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                var data = new NhanSuService(_dbcontext).getListForByNnId(nhanvienId)
                            .Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable()
                            .Select(o => new { o.ID, o.HoTen, o.ID_NhanVien, NgaySinh = convertDate(o.NgaySinh.ToString()), o.NoiO, o.QuanHe, o.DiaChi }).ToList();

                return Json(data);
            }
        }

        public IHttpActionResult GetQTDaotaoNv(Guid nhanvienId)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                var data = new NhanSuService(_dbcontext).getListNvQtDaoTaoForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.NoiHoc,
                    o.ID_NhanVien,
                    o.TuNgay,
                    o.DenNgay,
                    o.NganhHoc,
                    o.HeDaoTao,
                    o.BangCap

                }).ToList();
                return Json(data);
            }

        }

        public IHttpActionResult GetQTCongTacNv(Guid nhanvienId)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                var data = new NhanSuService(_dbcontext).getListNvQtCongTacForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.CoQuan,
                    o.ID_NhanVien,
                    o.TuNgay,
                    o.DenNgay,
                    o.DiaChi,
                    o.ViTri,

                }).ToList();
                return Json(data);
            }
        }

        public IHttpActionResult GetTTSucKhoeNv(Guid nhanvienId)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.getListNvTTSucKhoecForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.CanNang,
                    o.ID_NhanVien,
                    o.ChieuCao,
                    o.NgayKham,
                    o.TinhHinhSucKhoe,

                }).ToList();
                return Json(data);
            }

        }

        public IHttpActionResult GetHopDongNv(Guid nhanvienId)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(_dbcontext);
                var data = _NhanSuService.getListNvHopDongForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.LoaiHopDong,
                    TextLoaiHopDong = commonEnumHellper.ListLoaiHopDong.Any(c => c.Key.Equals(o.LoaiHopDong)) ? commonEnumHellper.ListLoaiHopDong.FirstOrDefault(c => c.Key.Equals(o.LoaiHopDong)).Value : string.Empty,
                    o.ID_NhanVien,
                    o.NgayKy,
                    o.ThoiHan,
                    o.DonViThoiHan,
                    o.SoHopDong,
                    ThoiHanText = _ClassNS_NhanVien.ConvertThoiHan(o.LoaiHopDong, o.ThoiHan, o.DonViThoiHan),
                    o.GhiChu

                }).ToList();
                return Json(data);
            }
        }



        public IHttpActionResult GetBaoHiemNv(Guid nhanvienId)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.getListNvBaoHiemForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.LoaiBaoHiem,
                    TextLoaiBaoHiem = commonEnumHellper.ListLoaiBaoHiem.Any(c => c.Key.Equals(o.LoaiBaoHiem)) ? commonEnumHellper.ListLoaiBaoHiem.FirstOrDefault(c => c.Key.Equals(o.LoaiBaoHiem)).Value : string.Empty,
                    o.ID_NhanVien,
                    o.NoiBaoHiem,
                    o.GhiChu,
                    o.SoBaoHiem,
                    o.NgayCap,
                    o.NgayHetHan

                }).ToList();
                return Json(data);
            }
        }

        public IHttpActionResult GetMienGiamNv(Guid nhanvienId)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.getListNvMienGiamForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.GhiChu,
                    o.ID_NhanVien,
                    o.KhoanMienGiam,
                    o.NgayApDung,
                    o.NgayKetThuc,
                    o.SoTien,

                }).ToList();
                return Json(data);
            }
        }

        public IHttpActionResult GetKhenThuongNv(Guid nhanvienId)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.getListNvKhenThuongForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.GhiChu,
                    o.ID_NhanVien,
                    o.HinhThuc,
                    o.NgayBanHang,
                    o.NoiDung,
                    o.SoQuyetDinh,

                }).ToList();
                return Json(data);
            }
        }

        public IHttpActionResult GetLuongNv(Guid nhanvienId)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.getListNvLuongForByNnId(nhanvienId).Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.SoTien,
                    o.HeSo,
                    o.Bac,
                    o.NoiDung,
                    o.NgayApDung,
                    o.NgayKetThuc,
                    o.ID_LoaiLuong,
                    o.ID_NhanVien,
                    o.LoaiLuong,
                    TenLoaiLuong = commonEnumHellper.ListLoaiLuongPhuCap.Any(c => c.Key == o.LoaiLuong) ? commonEnumHellper.ListLoaiLuongPhuCap.FirstOrDefault(c => c.Key == o.LoaiLuong).Value : string.Empty,

                }).ToList();
                return Json(data);
            }
        }

        public IHttpActionResult GetThietLapLuong_ofNhanVien(Guid idNhanVien, Guid idChiNhanh, int currentPage, int pageSize)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                List<ThietLapLuongDTO> data = _NhanSuService.GetThietLapLuong_ofNhanVien(idNhanVien, idChiNhanh, currentPage, pageSize);
                return Json(data);
            }
        }

        public IHttpActionResult GetTreePhongBan()
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetListPhongBanChiNhanh(null);
                return Json(data.ToList());
            }
        }

        public List<PhongBanChiNhanhView> GetPhongBanByChiNhanh(Guid chinhanhId)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetListPhongBanChiNhanh(chinhanhId);
                return data.ToList();
            }
        }

        public IHttpActionResult GetTreePhongBan(Guid chinhanhId)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetListPhongBanChiNhanh(chinhanhId);
                return Json(data.ToList());
            }
        }
        public IHttpActionResult GetBaoHienFilter()
        {
            return Json(commonEnumHellper.ListLoaiBaoHiem.Select(o => new { ID = o.Key, Name = o.Value, IsSelected = false }));
        }
        public IHttpActionResult GetChinhTriFilter()
        {
            return Json(commonEnumHellper.ListChinhTri.Select(o => new { ID = o.Key, Name = o.Value, IsSelected = false }));
        }
        public IHttpActionResult GetLOaiHopDongFilter()
        {
            return Json(commonEnumHellper.ListLoaiHopDong.Select(o => new { ID = o.Key, Name = o.Value, IsSelected = false }));
        }
        public IHttpActionResult GetDanTocFilter()
        {
            return Json(commonEnumHellper.ListDanToc.Select(o => new { ID = o.Key, Name = o.Value, IsSelected = false }));
        }
        public IHttpActionResult GetlistAllPhongBan()
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.getAllNvPhongban().Select(o => new { o.ID, o.TenPhongBan, o.ID_PhongBanCha }).ToList();
                return Json(data);
            }
        }
        public IHttpActionResult GetAllLoaiLuong()
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.getAllNvLoaiLuong().Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa)
                    // LoaiLuong: 5: phucap, 6: giamtru (defalt: LoaiPhuCap_GiamTru: 52: phucap codinh vnd, 62: giamtru codinh vnd)
                    .Select(o => new { o.ID, o.TenLoaiLuong, o.GhiChu, LoaiLuong = o.LoaiLuong, LoaiPhuCap_GiamTru = o.LoaiLuong == 5 ? 52 : 62 }).ToList();
                return Json(data);
            }
        }

        [HttpGet]
        public IHttpActionResult GetThietLapLuongChiTiet(Guid idLuongPC)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                try
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.GetThietLapLuongChiTiet().Where(o => o.ID_LuongPhuCap == idLuongPC)
                        .Select(o => new
                        {
                            o.ID,
                            o.ID_LuongPhuCap,
                            o.ID_CaLamViec,
                            o.LuongNgayThuong,
                            o.NgayThuong_LaPhanTramLuong,
                            o.Thu7_GiaTri,
                            o.Thu7_LaPhanTramLuong,
                            o.CN_LaPhanTramLuong,
                            o.ThCN_GiaTri,
                            o.NgayNghi_GiaTri,
                            o.NgayNghi_LaPhanTramLuong,
                            o.NgayLe_GiaTri,
                            o.NgayLe_LaPhanTramLuong,
                            o.LaOT,
                            TenCa = o.NS_CaLamViec == null ? "Mặc định" : o.NS_CaLamViec.TenCa,
                        }).OrderBy(x => x.ID_CaLamViec).ToList();
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return Exeption(ex);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GetListNhanVien_HadSetupSalary(Guid idChiNhanh)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                try
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.GetListNhanVien_HadSetupSalary(idChiNhanh);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return Exeption(ex);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GetListCaLamViec_ofDonVi(Guid idDonVi)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                try
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.GetListCaLamViec_ofDonVi(idDonVi);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return Exeption(ex);
                }
            }
        }

        public IHttpActionResult GetPrintHSNV(Guid? nhanvienId)
        {
            try
            {
                if (nhanvienId != null)
                {
                    var db = SystemDBContext.GetDBContext();

                    var data = new ClassNS_NhanVien(db).GetPrintNhanVien(nhanvienId, contant.GetUserCookies().ID_DonVi);
                    return ActionTrueData(data);

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
            return ActionFalseNotData("Lỗi không lấy được thông tin");
        }

        // chỉ get NhanVien trong phòng ban chưa cài đặt chiết khấu
        [HttpGet]
        public List<NS_NhanVien_DonVi> GetListNhanVien_inDepartment(Guid? idPhongBan, Guid idDonVi)
        {
            try
            {
                List<Guid?> lstID_PhongBan = new List<Guid?>();
                using (var db = SystemDBContext.GetDBContext())
                {
                    lstID_PhongBan = new ClassNS_NhanVien(db).GetlistIDPhongBanChild(lstID_PhongBan, idPhongBan ?? Guid.Empty);

                    lstID_PhongBan.Add(idPhongBan);
                    var sIDPhongBan = string.Join(",", lstID_PhongBan.ToArray());

                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_PhongBans", sIDPhongBan));
                    lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                    var data = db.Database.SqlQuery<NS_NhanVien_DonVi>("SP_GetListNhanVien_inDepartment @ID_PhongBans, @ID_DonVi", lstParam.ToArray()).ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetListNhanVien_inDepartment " + ex.InnerException + ex.Message);
            }
            return null;
        }



        private string convertDate(string input)
        {
            if (input.Length == 4)
            {
                return input.ToString();
            }
            else if (input.Length == 6)
            {
                return input.Substring(4, 2) + "/" + input.Substring(0, 4);
            }
            else if (input.Length == 8)
            {
                return input.Substring(6, 2) + "/" + input.Substring(4, 2) + "/" + input.Substring(0, 4);
            }
            return string.Empty;
        }
    }

    public class NS_NhanVienSelect
    {
        public Guid ID { get; set; }
        public string TenNhanVien { get; set; }
    }
    public class NS_NhanVienCaiDatChietKhau : NS_PhongBan_PROC
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string DienThoaiDiDong { get; set; }
        public string URLAnh { get; set; }
        public bool GioiTinh { get; set; }
    }
    public class NhanVien_ChiTiet_UpdateNhanVien
    {
        public Guid ID { get; set; }
        public double ChietKhau { get; set; }
        public double YeuCau { get; set; }
        public double TuVan { get; set; }
        public bool LaPhanTram { get; set; }
        public bool LaPhanTram_YeuCau { get; set; }
        public bool LaPhanTram_TuVan { get; set; }
    }
}

