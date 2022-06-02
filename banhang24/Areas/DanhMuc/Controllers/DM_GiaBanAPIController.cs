using banhang24.Compress;
using libDM_DoiTuong;
using libDM_GiaBan;
using libDM_GiaBan_ApDung;
using libDM_HangHoa;
using libDM_NhomHangHoa;
using libDonViQuiDoi;
using libHT_NguoiDung;
using libQuy_HoaDon;
//using System.Web.Mvc;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_GiaBanAPIController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        public List<DM_GiaBanSelect> GetDM_GiaBanByIDDonVi(Guid iddonvi)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (db == null)
                    {
                        return null;
                    }
                    else
                    {
                        var tbl = from gb in db.DM_GiaBan
                                  join gbad in db.DM_GiaBan_ApDung on gb.ID equals gbad.ID_GiaBan
                                  where gbad.ID_DonVi == null || gbad.ID_DonVi == iddonvi
                                  select new DM_GiaBanSelect
                                  {
                                      ID = gb.ID,
                                      TenGiaBan = gb.TenGiaBan
                                  };
                        return tbl.Distinct().ToList();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<DM_GiaBanSelect> GetDM_GiaBan_CoHieuLuc()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                List<DM_GiaBan> lstdata = _classDMGB.Gets(null);
                List<DM_GiaBanSelect> lst = new List<DM_GiaBanSelect>();

                DateTime _dtNow = DateTime.Now;
                DateTime _fromDate = _dtNow;
                DateTime _toDate = _dtNow;
                foreach (var item in lstdata)
                {
                    DM_GiaBanSelect select = new DM_GiaBanSelect();
                    _fromDate = item.TuNgay ?? _dtNow;
                    _toDate = item.DenNgay ?? _dtNow;
                    if (item.ApDung && DateTime.Compare(_dtNow, _fromDate) >= 0 && DateTime.Compare(_toDate, _dtNow) >= 0)
                    {
                        select.ID = item.ID;
                        select.TenGiaBan = item.TenGiaBan;
                        lst.Add(select);
                    }
                }
                return lst;
            }
        }

        [HttpGet, HttpPost]
        [Compress.DeflateCompression]
        public IHttpActionResult GetDMGiaBan_GBApDung_ChiTiet(Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var dtNow = DateTime.Now;

                List<DM_GiaBan> lstReturn = new List<DM_GiaBan>();
                if (db == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        var data = from gb in db.DM_GiaBan
                                   join ad in db.DM_GiaBan_ApDung on gb.ID equals ad.ID_GiaBan into GB_GBAD
                                   from gbap in GB_GBAD.DefaultIfEmpty()
                                   where gb.ApDung == true
                                    && (gb.TatCaDonVi == true || gbap.ID_DonVi == idDonVi)
                                    && DateTime.Compare(dtNow, gb.TuNgay ?? DateTime.Now) >= 0 && DateTime.Compare(gb.DenNgay ?? DateTime.Now, dtNow) >= 0
                                   group new { gbap } by new
                                   {
                                       gb.ID,
                                       gb.TenGiaBan,
                                       gb.TuNgay,
                                       gb.DenNgay,
                                       gb.TatCaDoiTuong,
                                       gb.TatCaDonVi,
                                       gb.TatCaNhanVien,
                                       gb.LoaiChungTuApDung,
                                       gb.NgayTrongTuan,
                                   };

                        foreach (var item in data)
                        {
                            DM_GiaBan itemGB = new DM_GiaBan();
                            itemGB.ID = item.Key.ID;
                            itemGB.TenGiaBan = item.Key.TenGiaBan;
                            itemGB.TuNgay = item.Key.TuNgay;
                            itemGB.DenNgay = item.Key.DenNgay;
                            itemGB.TatCaDonVi = item.Key.TatCaDonVi;
                            itemGB.TatCaDoiTuong = item.Key.TatCaDoiTuong;
                            itemGB.TatCaNhanVien = item.Key.TatCaNhanVien;
                            itemGB.LoaiChungTuApDung = item.Key.LoaiChungTuApDung;
                            itemGB.NgayTrongTuan = item.Key.NgayTrongTuan;

                            foreach (var itemGr in item)
                            {
                                DM_GiaBan_ApDung itemAD = new DM_GiaBan_ApDung();
                                if (itemGr.gbap != null)
                                {
                                    itemAD.ID_GiaBan = itemGB.ID;
                                    itemAD.ID_NhomKhachHang = itemGr.gbap.ID_NhomKhachHang ?? Guid.Empty;
                                    itemAD.ID_NhanVien = itemGr.gbap.ID_NhanVien ?? Guid.Empty;

                                    itemGB.DM_GiaBan_ApDung.Add(itemAD);
                                }
                            }
                            lstReturn.Add(itemGB);

                            var details = (from ct in db.DM_GiaBan_ChiTiet
                                           where ct.ID_GiaBan == item.Key.ID
                                           select new GiaBanChiTietBasicDTO
                                           {
                                               ID = ct.ID,
                                               ID_DonViQuiDoi = ct.ID_DonViQuiDoi,
                                               GiaBan = ct.GiaBan
                                           }).ToList();
                            itemGB.GiaBanChiTiet = details;
                        }
                        return Json(new { res = true, data = lstReturn });
                    }
                    catch (Exception e)
                    {
                        return Json(new { res = false, mes = e.InnerException + e.Message });
                    }
                }
            }
        }

        public List<DM_GiaBan> GetDMGiaBan_GBApDung(Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var dtNow = DateTime.Now;

                List<DM_GiaBan> lstReturn = new List<DM_GiaBan>();
                if (db == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        var data = from gb in db.DM_GiaBan
                                   join ad in db.DM_GiaBan_ApDung on gb.ID equals ad.ID_GiaBan into GB_GBAD
                                   from gbap in GB_GBAD.DefaultIfEmpty()
                                   where gb.ApDung == true
                                    && (gb.TatCaDonVi == true || gbap.ID_DonVi == idDonVi)
                                    && DateTime.Compare(dtNow, gb.TuNgay ?? DateTime.Now) >= 0 && DateTime.Compare(gb.DenNgay ?? DateTime.Now, dtNow) >= 0
                                   group new { gbap } by new
                                   {
                                       ID = gb.ID,
                                       TenGiaBan = gb.TenGiaBan,
                                       TuNgay = gb.TuNgay,
                                       DenNgay = gb.DenNgay,
                                       TatCaDoiTuong = gb.TatCaDoiTuong,
                                       TatCaDonVi = gb.TatCaDonVi,
                                       TatCaNhanVien = gb.TatCaNhanVien,
                                       LoaiChungTuApDung = gb.LoaiChungTuApDung,
                                       NgayTrongTuan = gb.NgayTrongTuan,
                                   };

                        foreach (var item in data)
                        {
                            DM_GiaBan itemGB = new DM_GiaBan();
                            itemGB.ID = item.Key.ID;
                            itemGB.TenGiaBan = item.Key.TenGiaBan;
                            itemGB.TuNgay = item.Key.TuNgay;
                            itemGB.DenNgay = item.Key.DenNgay;
                            itemGB.TatCaDonVi = item.Key.TatCaDonVi;
                            itemGB.TatCaDoiTuong = item.Key.TatCaDoiTuong;
                            itemGB.TatCaNhanVien = item.Key.TatCaNhanVien;
                            itemGB.LoaiChungTuApDung = item.Key.LoaiChungTuApDung;
                            itemGB.NgayTrongTuan = item.Key.NgayTrongTuan;

                            foreach (var itemGr in item)
                            {
                                DM_GiaBan_ApDung itemAD = new DM_GiaBan_ApDung();
                                if (itemGr.gbap != null)
                                {
                                    itemAD.ID_GiaBan = itemGB.ID;
                                    itemAD.ID_NhomKhachHang = itemGr.gbap.ID_NhomKhachHang ?? Guid.Empty;
                                    itemAD.ID_NhanVien = itemGr.gbap.ID_NhanVien ?? Guid.Empty;

                                    itemGB.DM_GiaBan_ApDung.Add(itemAD);
                                }
                            }
                            lstReturn.Add(itemGB);
                        }
                        return lstReturn;
                    }
                    catch (Exception e)
                    {
                        CookieStore.WriteLog("DM_GiaBanAPI_GetDMGiaBan_GBApDung: " + e.InnerException + e.Message);
                        return null;
                    }
                }
            }
        }

        public List<GiaBanChiTietDTO> GetListGiaBans(Guid id, Guid idGiaBan)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);

                List<DM_GiaBan_ChiTiet> lstAllGBs = _classDMGBCT.Gets(null);
                List<GiaBanChiTietDTO> lsrReturns = new List<GiaBanChiTietDTO>();
                if (lstAllGBs != null && lstAllGBs.Count() > 0)
                {
                    foreach (DM_GiaBan_ChiTiet item in lstAllGBs)
                    {
                        GiaBanChiTietDTO itemData = new GiaBanChiTietDTO();
                        itemData.ID = item.ID;
                        DonViQuiDoi dvqdtmp = _classDVQD.Get(p => p.ID == item.ID_DonViQuiDoi);
                        itemData.MaHangHoa = dvqdtmp.MaHangHoa;
                        itemData.TenHangHoa = _classDMHH.Get(p => p.ID == dvqdtmp.ID_HangHoa).TenHangHoa;
                        itemData.GiaVon = dvqdtmp.GiaVon;
                        itemData.GiaNhapCuoi = dvqdtmp.GiaVon;
                        itemData.GiaChung = item.GiaBan;
                        itemData.GiaMoi = item.GiaBan;
                        itemData.ID_GiaBan = item.ID_GiaBan;
                        itemData.ID_NhomHang = _classDMHH.Get(p => p.ID == dvqdtmp.ID_HangHoa).ID_NhomHang;
                        if (itemData.ID_NhomHang == id && itemData.ID_GiaBan == idGiaBan)
                        {
                            lsrReturns.Add(itemData);
                        }
                    };
                    return lsrReturns;
                }
                else
                    return null;
            }
        }

        public List<GiaBanChiTietDTO> GetListGiaBans1(Guid id, Guid idGiaBan)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                List<GiaBanChiTietDTO> lstAllGBs = _classDMGBCT.GetBangGia_NhomHang();
                List<GiaBanChiTietDTO> lstAll = new List<GiaBanChiTietDTO>();
                if (id == Guid.Empty)
                {
                    if (idGiaBan == Guid.Empty)
                    {
                        foreach (var item in lstAllGBs)
                        {
                            GiaBanChiTietDTO itemData = new GiaBanChiTietDTO();
                            itemData.ID = item.ID;
                            itemData.IDQuyDoi = item.IDQuyDoi;
                            itemData.MaHangHoa = item.MaHangHoa;
                            itemData.TenHangHoa = item.TenHangHoa;
                            itemData.GiaVon = item.GiaVon;
                            itemData.GiaNhapCuoi = item.GiaNhapCuoi;
                            itemData.GiaChung = item.GiaChung;
                            itemData.GiaMoi = item.GiaChung;
                            itemData.ID_GiaBan = Guid.Empty;
                            itemData.ID_NhomHang = item.ID_NhomHang;
                            itemData.DonViTinh = item.DonViTinh;
                            lstAll.Add(itemData);
                        };
                    }
                    else
                    {
                        foreach (var item in lstAllGBs)
                        {
                            //done
                            GiaBanChiTietDTO itemData = new GiaBanChiTietDTO();
                            itemData.ID = item.IDQuyDoi;
                            itemData.IDQuyDoi = item.IDQuyDoi;
                            itemData.MaHangHoa = item.MaHangHoa;
                            itemData.TenHangHoa = item.TenHangHoa;
                            itemData.GiaVon = item.GiaVon;
                            itemData.GiaNhapCuoi = item.GiaNhapCuoi;
                            itemData.GiaChung = item.GiaChung;
                            itemData.GiaMoi = item.GiaMoi;
                            itemData.ID_GiaBan = item.ID_GiaBan;
                            itemData.ID_NhomHang = item.ID_NhomHang;
                            itemData.DonViTinh = item.DonViTinh;
                            if (itemData.ID_GiaBan == idGiaBan)
                            {
                                lstAll.Add(itemData);
                            }
                        };
                    }
                }
                else
                {
                    if (idGiaBan == Guid.Empty)
                    {
                        foreach (var item in lstAllGBs)
                        {
                            //done
                            GiaBanChiTietDTO itemData = new GiaBanChiTietDTO();
                            itemData.ID = item.IDQuyDoi;
                            itemData.IDQuyDoi = item.IDQuyDoi;
                            itemData.MaHangHoa = item.MaHangHoa;
                            itemData.TenHangHoa = item.TenHangHoa;
                            itemData.GiaVon = item.GiaVon;
                            itemData.GiaNhapCuoi = item.GiaNhapCuoi;
                            itemData.GiaChung = item.GiaChung;
                            itemData.GiaMoi = item.GiaChung;
                            itemData.ID_GiaBan = item.ID_GiaBan;
                            itemData.ID_NhomHang = item.ID_NhomHang;
                            itemData.DonViTinh = item.DonViTinh;
                            if (itemData.ID_NhomHang == id)
                            {
                                lstAll.Add(itemData);
                            }
                        };
                    }
                    else
                    {
                        foreach (var item in lstAllGBs)
                        {
                            //done
                            GiaBanChiTietDTO itemData = new GiaBanChiTietDTO();
                            itemData.ID = item.ID;
                            itemData.IDQuyDoi = item.IDQuyDoi;
                            itemData.MaHangHoa = item.MaHangHoa;
                            itemData.TenHangHoa = item.TenHangHoa;
                            itemData.GiaVon = item.GiaVon;
                            itemData.GiaNhapCuoi = item.GiaNhapCuoi;
                            itemData.GiaChung = item.GiaChung;
                            itemData.GiaMoi = item.GiaMoi;
                            itemData.ID_GiaBan = item.ID_GiaBan;
                            itemData.ID_NhomHang = item.ID_NhomHang;
                            itemData.DonViTinh = item.DonViTinh;
                            if (itemData.ID_GiaBan == idGiaBan && itemData.ID_NhomHang == id)
                            {
                                lstAll.Add(itemData);
                            }
                        };
                    }
                }
                return lstAll;
            }
        }

        public List<GiaBanChiTietDTO> GetallGiaBan(string maHoaDon, string idnhomhang, string _id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                List<GiaBanChiTietDTO> lstAllGBs = _classDMGBCT.SelectChiTiet(_id);
                if (maHoaDon != null)
                {
                    lstAllGBs = lstAllGBs.Where(dt => dt.MaHangHoa.Contains(maHoaDon) || dt.TenHangHoa.Contains(maHoaDon)).ToList();
                }
                List<string> lstIDNHH = new List<string>();
                if (idnhomhang != null)
                {
                    var arrIDNHH = idnhomhang.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH.Add(arrIDNHH[i].ToString());
                    }
                }
                if (lstIDNHH.Count > 0)
                {
                    lstAllGBs = lstAllGBs.Where(hd => lstIDNHH.Contains(hd.ID_NhomHang.ToString())).ToList();
                }
                if (_id != "undefined")
                {
                    lstAllGBs = lstAllGBs.Where(hd => hd.ID_GiaBan.ToString().Contains(_id.ToString())).ToList();
                }
                return lstAllGBs.ToList();
            }
        }
        //Trinhpv xuất excel giá bán
        [HttpGet]
        public void ExportExcel_GiaBan(string maHoaDon, string idnhomhang, string _id, string columnsHide, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<GiaBanChiTietDTO> lstAllGBs = _classDMGBCT.SelectChiTiet_where(_id, maHoaDon, iddonvi);
                if (maHoaDon != null)
                {
                    lstAllGBs = lstAllGBs.Where(dt => dt.MaHangHoa.Contains(maHoaDon) || dt.TenHangHoa.Contains(maHoaDon)).ToList();
                }
                List<string> lstIDNHH = new List<string>();
                if (idnhomhang != null)
                {
                    var arrIDNHH = idnhomhang.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH.Add(arrIDNHH[i].ToString());
                    }
                }
                if (lstIDNHH.Count > 0)
                {
                    lstAllGBs = lstAllGBs.Where(hd => lstIDNHH.Contains(hd.ID_NhomHang.ToString())).ToList();
                }
                if (_id != "undefined")
                {
                    lstAllGBs = lstAllGBs.Where(hd => hd.ID_GiaBan.ToString().Contains(_id.ToString())).ToList();
                }
                List<BH_GiaBan_Excel> lst = new List<BH_GiaBan_Excel>();
                foreach (var item in lstAllGBs)
                {
                    BH_GiaBan_Excel DM = new BH_GiaBan_Excel();
                    DM.MaHangHoa = item.MaHangHoa;
                    DM.TenHangHoa = item.TenHangHoaFull;
                    DM.TenDonViTinh = item.DonViTinh;
                    DM.TenNhomHangHoa = item.TenNhomHangHoa;
                    DM.GiaVon = item.GiaVon;
                    DM.GiaNhapCuoi = item.GiaNhapCuoi;
                    DM.GiaChung = item.GiaChung;
                    DM.GiaMoi = item.GiaMoi;
                    lst.Add(DM);
                }
                DataTable excel = _classOFDCM.ToDataTable<BH_GiaBan_Excel>(lst);
                if (_id == "undefined" || string.IsNullOrEmpty(_id))
                {
                    columnsHide = "6";// remove colum GiaChung
                }
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhMucGiaBan.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhMucGiaBan.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, false, columnsHide);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }
        public System.Web.Http.Results.JsonResult<JsonResultExampleBangGia> GetListGiaBans_where(int currentPage, int pageSize, string maHoaDon, string idnhomhang, string _id, string columsort, string sort, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                List<GiaBanChiTietDTO> lsrReturns = new List<GiaBanChiTietDTO>();
                IEnumerable<GiaBanChiTietDTO> lstAllGBs = _classDMGBCT.SelectChiTiet_where(_id, maHoaDon, iddonvi);
                List<string> lstIDNHH = new List<string>();
                if (idnhomhang != null)
                {
                    var arrIDNHH = idnhomhang.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH.Add(arrIDNHH[i].ToString());
                    }
                }
                if (lstIDNHH.Count > 0)
                {
                    lstAllGBs = lstAllGBs.Where(hd => lstIDNHH.Contains(hd.ID_NhomHang.ToString()));
                }
                double round = lstAllGBs.Count();
                var pageCount = System.Math.Ceiling(round / pageSize);
                if (sort != null)
                {
                    if (sort == "0")
                    {
                        if (columsort == "MaHang")
                        {
                            lstAllGBs = lstAllGBs.OrderBy(p => p.MaHangHoa).ToList();
                        }
                        if (columsort == "TenHang")
                        {
                            lstAllGBs = lstAllGBs.OrderBy(p => p.TenHangHoa).ToList();
                        }
                        if (columsort == "NhomHang")
                        {
                            lstAllGBs = lstAllGBs.OrderBy(p => p.TenNhomHangHoa).ToList();
                        }
                        if (columsort == "GiaVon")
                        {
                            lstAllGBs = lstAllGBs.OrderBy(p => p.GiaVon).ToList();
                        }
                        if (columsort == "GiaNhapCuoi")
                        {
                            lstAllGBs = lstAllGBs.OrderBy(p => p.GiaNhapCuoi).ToList();
                        }
                        if (columsort == "GiaChung")
                        {
                            lstAllGBs = lstAllGBs.OrderBy(p => p.GiaChung).ToList();
                        }
                    }
                    else
                    {
                        if (columsort == "MaHang")
                        {
                            lstAllGBs = lstAllGBs.OrderByDescending(p => p.MaHangHoa).ToList();
                        }
                        if (columsort == "TenHang")
                        {
                            lstAllGBs = lstAllGBs.OrderByDescending(p => p.TenHangHoa).ToList();
                        }
                        if (columsort == "NhomHang")
                        {
                            lstAllGBs = lstAllGBs.OrderByDescending(p => p.TenNhomHangHoa).ToList();
                        }
                        if (columsort == "GiaVon")
                        {
                            lstAllGBs = lstAllGBs.OrderByDescending(p => p.GiaVon).ToList();
                        }
                        if (columsort == "GiaNhapCuoi")
                        {
                            lstAllGBs = lstAllGBs.OrderByDescending(p => p.GiaNhapCuoi).ToList();
                        }
                        if (columsort == "GiaChung")
                        {
                            lstAllGBs = lstAllGBs.OrderByDescending(p => p.GiaChung).ToList();
                        }
                    }
                }
                lstAllGBs = lstAllGBs.Skip(currentPage * pageSize).Take(pageSize);
                foreach (GiaBanChiTietDTO item in lstAllGBs)
                {
                    GiaBanChiTietDTO gbct = new GiaBanChiTietDTO
                    {
                        ID = item.ID,
                        IDQuyDoi = item.IDQuyDoi,
                        ID_GiaBan = item.ID_GiaBan,
                        ID_HangHoa = item.ID_HangHoa,
                        ID_NhomHang = item.ID_NhomHang,
                        TenNhomHangHoa = item.TenNhomHangHoa,
                        TenHangHoa = item.TenHangHoa,
                        //NgayTao = item.NgayTao,
                        //Xoa = item.Xoa,
                        DonViTinh = item.DonViTinh,
                        GiaNhapCuoi = Math.Round((double)item.GiaNhapCuoi, MidpointRounding.ToEven),
                        GiaMoi = Math.Round((double)item.GiaMoi, MidpointRounding.ToEven),
                        MaHangHoa = item.MaHangHoa,
                        GiaVon = Math.Round((double)item.GiaVon, MidpointRounding.ToEven),
                        GiaBan = Math.Round((double)item.GiaBan, MidpointRounding.ToEven),
                        GiaChung = Math.Round((double)item.GiaChung, MidpointRounding.ToEven),
                        QuanLyTheoLoHang = item.QuanLyTheoLoHang,
                        HangHoaThuocTinh = item.HangHoaThuocTinh
                    };
                    lsrReturns.Add(gbct);
                }
                JsonResultExampleBangGia jsonobj = new JsonResultExampleBangGia
                {
                    Rowcount = round,
                    pageCount = pageCount,
                    lstBG = lsrReturns.ToList(),
                    //TongTon = tongton
                };
                return Json(jsonobj);
            }
        }

        public PageListDTO GetPageCountHoaDon_Where(int currentPage, float pageSize, string maHoaDon, string idnhomhang, string _id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                var totalRecords = 0;
                IEnumerable<GiaBanChiTietDTO> lstAllGBs = _classDMGBCT.SelectChiTiet_where(_id, maHoaDon, iddonvi);
                //if (maHoaDon != null)
                //{
                //    lstAllGBs = lstAllGBs.Where(dt => dt.MaHangHoa.Contains(maHoaDon) || dt.TenHangHoa.Contains(maHoaDon));
                //}
                List<string> lstIDNHH = new List<string>();
                if (idnhomhang != null)
                {
                    var arrIDNHH = idnhomhang.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH.Add(arrIDNHH[i].ToString());
                    }
                }
                if (lstIDNHH.Count > 0)
                {
                    lstAllGBs = lstAllGBs.Where(hd => lstIDNHH.Contains(hd.ID_NhomHang.ToString()));
                }
                if (lstAllGBs != null)
                {
                    totalRecords = lstAllGBs.Count();
                }
                PageListDTO pageListDTO = new PageListDTO
                {
                    TotalRecord = totalRecords,
                    PageCount = System.Math.Ceiling(totalRecords / pageSize) // round 6.1 --> 7
                };
                return pageListDTO;
            }
        }

        public double GetPageCountHoaDon(float pageSize)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                var totalRecord = _classDMGBCT.GetBangGia_NhomHang().Count();
                // round 6.1 --> 7
                return System.Math.Ceiling(totalRecord / pageSize);
            }
        }

        public double GetTotalRecord()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                return _classDMGBCT.GetBangGia_NhomHang().Count();
            }
        }

        public List<GiaBanChiTietDTO> GetChiTietGiaBan(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                return _classDMGBCT.SelectChiTiet(id).ToList();
            }
        }

        [HttpGet]
        public string AddChiTiet(string idnhomhanghoa, Guid idgiaban)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (idgiaban != Guid.Empty)
                {
                    if (idnhomhanghoa == null)
                    {
                        idnhomhanghoa = "%%";
                    }
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("ListID_NhomHang", idnhomhanghoa));
                    paramlist.Add(new SqlParameter("ID_GiaBan", idgiaban));
                    paramlist.Add(new SqlParameter("ID_KhoHang", new Guid("01CD02F2-4612-4104-B790-1C0373CBD72D")));
                    paramlist.Add(new SqlParameter("ID_NgoaiTe", new Guid("406eed2d-faae-4520-aef2-12912f83dda2")));
                    db.Database.ExecuteSqlCommand("exec AddChiTietGia @ListID_NhomHang, @ID_GiaBan, @ID_KhoHang, @ID_NgoaiTe", paramlist.ToArray());
                    return "";
                }
                else
                {
                    return "Error";
                }
            }
        }

        [HttpGet]
        public bool AddChiTietHang(Guid iddonviqd, Guid idgiaban)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                return _classDMGBCT.AddChiTietByIDhang(iddonviqd, idgiaban);
            }
        }

        [HttpGet]
        public string deleteChiTiet(Guid idgiaban)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("ID_GiaBan", idgiaban));
                    db.Database.ExecuteSqlCommand("exec XoaChiTietbangGia @ID_GiaBan", paramlist.ToArray());
                    return "";
                }
                else
                {
                    return "Error";
                }
            }
        }

        [ResponseType(typeof(DM_GiaBan))]
        public IHttpActionResult GetDM_GiaBan(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                DM_GiaBan giaban = _classDMGB.Select_GiaBan(id);
                DM_GiaBan temp = new DM_GiaBan();
                temp.ID = giaban.ID;
                temp.TenGiaBan = giaban.TenGiaBan;
                temp.TuNgay = giaban.TuNgay;
                temp.DenNgay = giaban.DenNgay;
                temp.ApDung = giaban.ApDung;
                temp.GhiChu = giaban.GhiChu;
                temp.TatCaDoiTuong = giaban.TatCaDoiTuong;
                temp.TatCaDonVi = giaban.TatCaDonVi;
                temp.TatCaNhanVien = giaban.TatCaNhanVien;
                if (giaban == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(temp);
                }
            }
        }

        public List<DM_GiaBanDTO> GetGiaBan_ApDung(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                List<DM_GiaBanDTO> lstAllHHs = _classDMGB.selectallGiaBanAD(id);
                if (lstAllHHs.Count > 0)
                {
                    return lstAllHHs;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<DM_DonVi> getLisDonViGB(Guid id_giaban)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                List<DM_DonVi> lst = _classDMGB.getLisDonViGB(id_giaban);
                return lst;
            }
        }

        public List<NS_NhanVien> getlistNhanVienBG(Guid id_giaban)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                List<NS_NhanVien> lst = _classDMGB.getlistNhanVienBG(id_giaban);
                return lst;
            }
        }

        public List<DM_NhomDoiTuong> getlistNhomKHangBG(Guid id_giaban)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                List<DM_NhomDoiTuong> lst = _classDMGB.getlistNhomKHangBG(id_giaban);
                return lst;
            }
        }

        [HttpPost, ActionName("PostGiaBan")]
        public IHttpActionResult PostGiaBan([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                classGiaBan_ApDung _classGBAD = new classGiaBan_ApDung(db);
                DM_GiaBan objGiaBan = data["objNewGiaBan"].ToObject<DM_GiaBan>();
                List<DM_GiaBan_ApDung> objGiaBanApDung = new List<DM_GiaBan_ApDung>();
                if (data["objGiaBanApDung"] != null)
                {
                    objGiaBanApDung = data["objGiaBanApDung"].ToObject<List<DM_GiaBan_ApDung>>();
                }

                string _tengiaban = objGiaBan.TenGiaBan;
                if (_classDMGB.Select_GiaBan(_tengiaban, null) != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Tên bảng giá đã được sử dụng. Hãy nhập tên khác!"));
                }
                #region add danh mục giá bán
                DM_GiaBan dmgiaban = new DM_GiaBan();
                dmgiaban.ID = Guid.NewGuid();
                dmgiaban.ApDung = objGiaBan.ApDung;
                dmgiaban.DenNgay = objGiaBan.DenNgay;
                dmgiaban.GhiChu = objGiaBan.GhiChu;
                dmgiaban.NgayTao = DateTime.Now;
                dmgiaban.NguoiTao = "ADMIN";
                dmgiaban.TatCaDoiTuong = objGiaBan.TatCaDoiTuong;
                dmgiaban.TatCaDonVi = objGiaBan.TatCaDonVi;
                dmgiaban.TatCaNhanVien = objGiaBan.TatCaNhanVien;
                dmgiaban.TenGiaBan = objGiaBan.TenGiaBan;
                dmgiaban.TuNgay = objGiaBan.TuNgay;
                dmgiaban.NgayTrongTuan = objGiaBan.NgayTrongTuan;
                dmgiaban.LoaiChungTuApDung = objGiaBan.LoaiChungTuApDung;
                string strErr = _classDMGB.AddDM_GiaBan(dmgiaban);

                if (objGiaBanApDung.Count == 0)
                {
                    DM_GiaBan_ApDung dmgiabanapdung = new DM_GiaBan_ApDung();
                    dmgiabanapdung.ID = Guid.NewGuid();
                    dmgiabanapdung.ID_GiaBan = dmgiaban.ID;
                    dmgiabanapdung.ID_DonVi = null;
                    dmgiabanapdung.ID_NhanVien = null;
                    dmgiabanapdung.ID_NhomKhachHang = null;
                    _classGBAD.Add_GBApDung(dmgiabanapdung);
                }
                foreach (var item in objGiaBanApDung)
                {
                    DM_GiaBan_ApDung dmgiabanapdung = new DM_GiaBan_ApDung();
                    dmgiabanapdung.ID = Guid.NewGuid();
                    dmgiabanapdung.ID_GiaBan = dmgiaban.ID;
                    dmgiabanapdung.ID_DonVi = item.ID_DonVi;
                    dmgiabanapdung.ID_NhanVien = item.ID_NhanVien;
                    dmgiabanapdung.ID_NhomKhachHang = item.ID_NhomKhachHang;
                    _classGBAD.Add_GBApDung(dmgiabanapdung);
                }
                if (strErr == string.Empty)
                {
                    DM_GiaBanDTO objReturn = new DM_GiaBanDTO
                    {
                        ID = dmgiaban.ID,
                        ApDung = dmgiaban.ApDung,
                        DenNgay = dmgiaban.DenNgay,
                        GhiChu = dmgiaban.GhiChu,
                        NgayTao = dmgiaban.NgayTao,
                        NguoiTao = dmgiaban.NguoiTao,
                        TatCaDoiTuong = dmgiaban.TatCaDoiTuong,
                        TatCaDonVi = dmgiaban.TatCaDonVi,
                        TatCaNhanVien = dmgiaban.TatCaNhanVien,
                        TenGiaBan = dmgiaban.TenGiaBan,
                        TuNgay = dmgiaban.TuNgay,
                        NgayTrongTuan = dmgiaban.NgayTrongTuan,
                        LoaiChungTuApDung = dmgiaban.LoaiChungTuApDung
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strErr));
                }
                #endregion
            }
        }

        [HttpPost, ActionName("PutGiaBan")]
        public IHttpActionResult PutGiaBan([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                classGiaBan_ApDung _classGBAD = new classGiaBan_ApDung(db);
                DM_GiaBan objGiaBan = data["objNewGiaBan"].ToObject<DM_GiaBan>();
                List<DM_GiaBan_ApDung> objGiaBanApDung = new List<DM_GiaBan_ApDung>();
                if (data["objGiaBanApDung"] != null)
                {
                    objGiaBanApDung = data["objGiaBanApDung"].ToObject<List<DM_GiaBan_ApDung>>();
                }
                if (_classDMGB.Select_GiaBan(objGiaBan.TenGiaBan, objGiaBan.ID) != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Tên bảng giá đã được sử dụng. Hãy nhập tên khác"));
                }

                DM_GiaBan dmgiaban = new DM_GiaBan();
                dmgiaban.ID = objGiaBan.ID;
                dmgiaban.ApDung = objGiaBan.ApDung;
                dmgiaban.DenNgay = objGiaBan.DenNgay;
                dmgiaban.GhiChu = objGiaBan.GhiChu;
                dmgiaban.NgaySua = DateTime.Now;
                dmgiaban.NguoiSua = "ADMIN";
                dmgiaban.TatCaDoiTuong = objGiaBan.TatCaDoiTuong;
                dmgiaban.TatCaDonVi = objGiaBan.TatCaDonVi;
                dmgiaban.TatCaNhanVien = objGiaBan.TatCaNhanVien;
                dmgiaban.TenGiaBan = objGiaBan.TenGiaBan;
                dmgiaban.TuNgay = objGiaBan.TuNgay;
                dmgiaban.NgayTrongTuan = objGiaBan.NgayTrongTuan;
                dmgiaban.LoaiChungTuApDung = objGiaBan.LoaiChungTuApDung;
                string strErr = _classDMGB.UpdateDM_GiaBan(dmgiaban);

                db.DM_GiaBan_ApDung.RemoveRange(db.DM_GiaBan_ApDung.Where(idDV => idDV.ID_GiaBan == objGiaBan.ID));
                db.SaveChanges();
                if (objGiaBanApDung.Count == 0)
                {
                    DM_GiaBan_ApDung dmgiabanapdung = new DM_GiaBan_ApDung();
                    dmgiabanapdung.ID = Guid.NewGuid();
                    dmgiabanapdung.ID_GiaBan = dmgiaban.ID;
                    dmgiabanapdung.ID_DonVi = null;
                    dmgiabanapdung.ID_NhanVien = null;
                    dmgiabanapdung.ID_NhomKhachHang = null;
                    _classGBAD.Add_GBApDung(dmgiabanapdung);
                }

                foreach (var item in objGiaBanApDung)
                {
                    DM_GiaBan_ApDung dmgiabanapdung = new DM_GiaBan_ApDung();
                    if (item.ID == Guid.Empty)
                    {
                        dmgiabanapdung.ID = Guid.NewGuid();
                    }
                    else
                    {
                        dmgiabanapdung.ID = item.ID;
                    }
                    dmgiabanapdung.ID_GiaBan = dmgiaban.ID;
                    dmgiabanapdung.ID_DonVi = item.ID_DonVi;
                    dmgiabanapdung.ID_NhanVien = item.ID_NhanVien;
                    dmgiabanapdung.ID_NhomKhachHang = item.ID_NhomKhachHang;
                    string strErrAD = _classGBAD.Add_GBApDung(dmgiabanapdung);
                }

                if (strErr == string.Empty)
                {
                    DM_GiaBanDTO objReturn = new DM_GiaBanDTO
                    {
                        ID = dmgiaban.ID,
                        ApDung = dmgiaban.ApDung,
                        DenNgay = dmgiaban.DenNgay,
                        GhiChu = dmgiaban.GhiChu,
                        NgayTao = dmgiaban.NgayTao,
                        NguoiTao = dmgiaban.NguoiTao,
                        TatCaDoiTuong = dmgiaban.TatCaDoiTuong,
                        TatCaDonVi = dmgiaban.TatCaDonVi,
                        TatCaNhanVien = dmgiaban.TatCaNhanVien,
                        TenGiaBan = dmgiaban.TenGiaBan,
                        TuNgay = dmgiaban.TuNgay,
                        NgayTrongTuan = dmgiaban.NgayTrongTuan,
                        LoaiChungTuApDung = dmgiaban.LoaiChungTuApDung
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strErr));
                }
            }
        }

        [HttpPost, ActionName("PutGiaBanChiTiet")]
        public IHttpActionResult PutGiaBanChiTiet([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                //Guid id = data["id"].ToObject<Guid>();
                List<GiaBan_ChiTiet_UpdateGiaBan> objList = data["objData"].ToObject<List<GiaBan_ChiTiet_UpdateGiaBan>>();
                string strUpd = "";
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                foreach (var item in objList)
                {
                    strUpd = _classDMGBCT.Update_GiaBanCT(item.ID, item.GiaBan);
                }
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
        }

        [HttpPost, ActionName("PutGiaBanChiTietChung")]
        public IHttpActionResult PutGiaBanChiTietChung([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                List<DM_GiaBan_ChiTiet> objList = data["objData"].ToObject<List<DM_GiaBan_ChiTiet>>();
                string strUpd = "";
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                foreach (var item in objList)
                {
                    strUpd = _classDMGBCT.Update_GiaBanCTChung(item.ID, item.GiaBan);
                }
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietChungCongVND(int LoaiGiaChon, double giaTri, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietChungCongVND @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietChungCongPhanTram(int LoaiGiaChon, double giaTri, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietChungCongPhanTram @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietChungTruVND(int LoaiGiaChon, double giaTri, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietChungTruVND @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietChungTruPhanTram(int LoaiGiaChon, double giaTri, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietChungTruPhanTram @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietCongVND(int LoaiGiaChon, double giaTri, Guid id_giaban, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("ID", id_giaban));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietCongVND @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ID, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietCongPhanTram(int LoaiGiaChon, double giaTri, Guid id_giaban, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("ID", id_giaban));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietCongPhanTram @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ID, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietTruVND(int LoaiGiaChon, double giaTri, Guid id_giaban, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("ID", id_giaban));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietTruVND @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ID, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpPost]
        public string PutGiaBanChiTietTruPhanTram(int LoaiGiaChon, double giaTri, Guid id_giaban, string idnhomhang, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (idnhomhang == null)
                {
                    idnhomhang = "";
                }
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("LoaiGiaChon", LoaiGiaChon));
                paramlist.Add(new SqlParameter("giaTri", giaTri));
                paramlist.Add(new SqlParameter("ID", id_giaban));
                paramlist.Add(new SqlParameter("@ListID_NhomHang", idnhomhang));
                db.Database.ExecuteSqlCommand("exec PutGiaBanChiTietTruPhanTram @ID_ChiNhanh, @LoaiGiaChon, @giaTri, @ID, @ListID_NhomHang", paramlist.ToArray());
                return "";
            }
        }

        [HttpGet]
        public bool CheckBangGia_wasUse(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    return db.BH_HoaDon.Where(x => x.ID_BangGia == id).Count() > 0;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        #region delete
        [HttpDelete, HttpGet]
        public IHttpActionResult DeleteDM_GiaBan(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                    _classDMGB.Delete_GiaBan(id);
                    return Json(new { res = true });
                }
                catch (Exception ex)
                {
                    return Json(new { res = false, mes = ex.InnerException });
                }
            }
        }

        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult DeleteAChiTietGiaBan(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_GiaBan _classDMGB = new classDM_GiaBan(db);
                string strDel = _classDMGB.DeleteAChiTietGiaBan(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion
    }

    public class DM_GiaBanSelect
    {
        public Guid ID { get; set; }
        public string TenGiaBan { get; set; }
    }

    public class GiaBan_ChiTiet_UpdateGiaBan
    {
        public Guid ID { get; set; }
        public double GiaBan { get; set; }
    }

    public class JsonResultExampleBangGia
    {
        public double Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<GiaBanChiTietDTO> lstBG { get; set; }
    }
}
