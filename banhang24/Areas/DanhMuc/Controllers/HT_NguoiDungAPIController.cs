using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using libHT_NguoiDung;
using System.Web.Http.Description;
using Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;
using libDM_DoiTuong;
using libDM_DonVi;
using System.Data.Entity;
using libNS_NhanVien;
using Model_banhang24vn.DAL;
using HT;
using libHT;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class HT_NguoiDungAPIController : BaseApiController
    {
        #region GET
        // GET: api/DanhMuc/HT_NguoiDungAPI/GetHT_NGuoiDung
        public IQueryable<HT_NguoiDung> GetHT_NGuoiDung()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.Gets(null);
            }
        }

        public List<HT_NguoiDungDTO> GetListNguoiDung_where(int currentPage, int pageSize, string maHoaDon, string idnhomnguoidung, int trangthai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                List<HT_NguoiDungDTO> lstAllVTs = _classND.getAllNguoiDung_where(maHoaDon, idnhomnguoidung, trangthai);
                return lstAllVTs;
            }
        }

        public List<HT_NguoiDungDTO> GetListNguoiDung()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                List<HT_NguoiDungDTO> lstAllVTs = _classND.getAllNguoiDung().ToList();
                return lstAllVTs;
            }
        }

        public PageListDTO GetPageCountND_Where(int currentPage, float pageSize, string maHoaDon, string idnhomnguoidung, int trangthai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                var totalRecords = 0;
                var data = _classND.getAllNguoiDung_where(maHoaDon, idnhomnguoidung, trangthai);
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

        public List<HT_NguoiDungDTO> changeHTNguoiDungByIDNhom(Guid idnhomnd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                List<HT_NguoiDungDTO> lstAllVTs = _classND.changeHTNguoiDungByIDNhom(idnhomnd).ToList();
                return lstAllVTs;
            }
        }

        public List<HT_NguoiDungDTO> getallNDByID_DonVi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                List<HT_NguoiDungDTO> lstNDDV = _classND.getNguoiDungByID_DonVi(id).ToList();
                return lstNDDV;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool GioiHanSoNguoiDung()
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            int gioihan = CuaHangDangKyService.GetGioiHanNguoiDung(str);
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<HT_NguoiDung> lst = db.HT_NguoiDung.Where(x => x.DangHoatDong).ToList();
                if (gioihan == 0)
                {
                    return false;
                }
                else
                {
                    if (lst.Count() >= gioihan)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public List<HT_NhomNguoiDungDTO> GetListVaiTro()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                List<HT_NhomNguoiDungDTO> lstAllVTs = _classNND.getallvaitro().ToList();
                return lstAllVTs;
            }
        }

        [HttpGet]
        public string NgungHoatDong(Guid id, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    HT_NguoiDung nd = _classND.Get(p => p.ID == id);
                    if (nd != null)
                    {
                        nd.DangHoatDong = false;
                        _classND.Update_NguoiDung(nd);
                        HT_NhatKySuDung nhatKySuDung = new HT_NhatKySuDung();
                        nhatKySuDung.ID = Guid.NewGuid();
                        nhatKySuDung.ID_NhanVien = idnhanvien;
                        nhatKySuDung.ChucNang = "Người dùng";
                        nhatKySuDung.ThoiGian = DateTime.Now;
                        nhatKySuDung.NoiDung = "Ngừng hoạt động người dùng" + nd.TaiKhoan;
                        nhatKySuDung.NoiDungChiTiet = "Ngừng hoạt động người dùng" + nd.TaiKhoan;
                        nhatKySuDung.LoaiNhatKy = 2;
                        nhatKySuDung.ID_DonVi = iddonvi;
                        SaveDiary.add_Diary(nhatKySuDung);

                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }

        [HttpGet]
        public string ChoHoatDong(Guid id, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    HT_NguoiDung nd = _classND.Get(p => p.ID == id);
                    if (nd != null)
                    {
                        nd.DangHoatDong = true;
                        _classND.Update_NguoiDung(nd);

                        HT_NhatKySuDung nhatKySuDung = new HT_NhatKySuDung();
                        nhatKySuDung.ID = Guid.NewGuid();
                        nhatKySuDung.ID_NhanVien = idnhanvien;
                        nhatKySuDung.ChucNang = "Người dùng";
                        nhatKySuDung.ThoiGian = DateTime.Now;
                        nhatKySuDung.NoiDung = "Cho phép hoạt động người dùng" + nd.TaiKhoan;
                        nhatKySuDung.NoiDungChiTiet = "Cho phép hoạt động người dùng" + nd.TaiKhoan;
                        nhatKySuDung.LoaiNhatKy = 2;
                        nhatKySuDung.ID_DonVi = iddonvi;
                        SaveDiary.add_Diary(nhatKySuDung);
                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }

        [ResponseType(typeof(HT_NguoiDungDTO))]
        public IHttpActionResult GetHT_NguoiDung(Guid idnguoidung)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                classDM_DonVi _classDV = new classDM_DonVi(db);
                HT_NguoiDung hT_NguoiDung = _classND.Select_HTNguoiDung(idnguoidung);
                //HT_NguoiDung_Nhom hT_Nhom_NguoiDung = classHT_NguoiDung_Nhom.Get(p=>p.IDNguoiDung == idnguoidung && p.ID_DonVi == iddonvind);
                HT_NguoiDungDTO ct = new HT_NguoiDungDTO();
                ct.ID = hT_NguoiDung.ID;
                ct.TaiKhoan = hT_NguoiDung.TaiKhoan;
                ct.MatKhau = hT_NguoiDung.MatKhau;
                ct.ID_NhanVien = hT_NguoiDung.ID_NhanVien;
                ct.ID_DonVi = hT_NguoiDung.ID_DonVi;
                ct.LaAdmin = hT_NguoiDung.LaAdmin;
                //ct.IDNhomNguoiDung = hT_Nhom_NguoiDung.IDNhomNguoiDung;
                ct.TenDonVi = _classDV.Get(p => p.ID == hT_NguoiDung.ID_DonVi).TenDonVi;
                if (hT_NguoiDung == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        public List<HT_NguoiDung_NhomDTO> GetListVaiTro(Guid? idnhanvien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                classHT_NguoiDung_Nhom _classNDN = new classHT_NguoiDung_Nhom(db);
                HT_NguoiDung htnd = _classND.Get(p => p.ID_NhanVien == idnhanvien);
                List<HT_NguoiDung_NhomDTO> lst = new List<HT_NguoiDung_NhomDTO>();
                if (htnd != null)
                {
                    List<HT_NguoiDung_Nhom> htndnhom = _classNDN.Gets(p => p.IDNguoiDung == htnd.ID).ToList();
                    foreach (var item in htndnhom)
                    {
                        HT_NguoiDung_NhomDTO nhomnd = new HT_NguoiDung_NhomDTO();
                        nhomnd.ID_DonVi = item.ID_DonVi.Value;
                        lst.Add(nhomnd);
                    }
                }
                return lst;
            }
        }


        [ResponseType(typeof(HT_NhomNguoiDungDTO))]
        public IHttpActionResult GetHT_NhomNguoiDung(Guid idnguoidung, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                classHT_NguoiDung_Nhom _classNDN = new classHT_NguoiDung_Nhom(db);
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                classHT_Quyen _classHTQuyen = new classHT_Quyen(db);

                HT_NguoiDung ngdung = _classND.Get(p => p.ID == idnguoidung);
                HT_NhomNguoiDungDTO ct = new HT_NhomNguoiDungDTO();
                if (ngdung.LaAdmin == false)
                {
                    HT_NguoiDung_Nhom nguoidungnhom = _classNDN.Gets(p => p.IDNguoiDung == idnguoidung && p.ID_DonVi == iddonvi).FirstOrDefault();
                    if (nguoidungnhom != null)
                    {
                        var htqn = _classND.Select_HT_Quyen_Nhom(nguoidungnhom.IDNhomNguoiDung);
                        HT_NhomNguoiDung hT_NhomNguoiDung = _classNND.Select_NhomNguoiDung(nguoidungnhom.IDNhomNguoiDung);

                        ct.ID = hT_NhomNguoiDung.ID;
                        ct.TenNhom = hT_NhomNguoiDung.TenNhom;
                        List<HT_Quyen_NhomDTO> HT_QN = htqn.Select(s => new HT_Quyen_NhomDTO
                        {
                            ID = s.ID,
                            MaQuyen = s.MaQuyen,
                            //ID_NhomNguoiDung = s.ID_NhomNguoiDung
                        }).ToList();
                        ct.HT_Quyen_NhomDTO = HT_QN;
                    }
                    else
                    {
                        ct.HT_Quyen_NhomDTO = new List<HT_Quyen_NhomDTO>();
                    }
                }
                else
                {
                    var htqn = _classHTQuyen.Gets(null);
                    List<HT_Quyen_NhomDTO> HT_QN = htqn.Select(s => new HT_Quyen_NhomDTO
                    {
                        MaQuyen = s.MaQuyen,
                        //ID_NhomNguoiDung = s.ID_NhomNguoiDung
                    }).ToList();
                    ct.HT_Quyen_NhomDTO = HT_QN;
                }
                return Ok(ct);
            }
        }

        // because not reload page BanHang --> not reload ID_nguoiDung: so get from Cookies
        [banhang24.Compress.DeflateCompression]
        [ResponseType(typeof(List<HT_NhomNguoiDungDTO>))]
        public IHttpActionResult GetListQuyen_OfNguoiDung()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                var nv_nd = banhang24.Hellper.contant.GetUserCookies();
                var idnguoidung = nv_nd.ID.ToString();
                var iddonvi = nv_nd.ID_DonVi.ToString();
                var ct = _classNND.SP_GetQuyen_ByIDNguoiDung(idnguoidung, iddonvi);
                if (ct != null)
                {
                    return Ok(ct);
                }
                else
                {
                    return Json("");
                }
            }
        }

        [ResponseType(typeof(HT_NhomNguoiDungDTO))]
        public IHttpActionResult GetHT_NguoiDung_Nhom(Guid idnguoidung, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung_Nhom _classNDN = new classHT_NguoiDung_Nhom(db);
                HT_NguoiDung_Nhom nguoidungnhom = _classNDN.Gets(p => p.IDNguoiDung == idnguoidung && p.ID_DonVi == iddonvi).FirstOrDefault();
                HT_NguoiDung_NhomDTO ct = new HT_NguoiDung_NhomDTO();
                ct.ID = nguoidungnhom.ID;
                ct.IDNhomNguoiDung = nguoidungnhom.IDNhomNguoiDung;
                ct.IDNguoiDung = nguoidungnhom.IDNguoiDung;
                ct.ID_DonVi = nguoidungnhom.ID_DonVi.Value;
                if (nguoidungnhom == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        [ResponseType(typeof(HT_NhomNguoiDungDTO))]
        public IHttpActionResult GetHT_NhomNguoiDungEDit(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                HT_NhomNguoiDung hT_NhomNguoiDung = _classNND.Select_NhomNguoiDung(id);
                HT_NhomNguoiDungDTO ct = new HT_NhomNguoiDungDTO();
                ct.ID = hT_NhomNguoiDung.ID;
                ct.TenNhom = hT_NhomNguoiDung.TenNhom;
                IQueryable<HT_Quyen_Nhom> htqn = _classND.Select_HT_Quyen_Nhom(id);
                List<HT_Quyen_NhomDTO> HT_QN = htqn.Select(s => new HT_Quyen_NhomDTO
                {
                    ID = s.ID,
                    MaQuyen = s.MaQuyen,
                    //ID_NhomNguoiDung = s.ID_NhomNguoiDung
                }).ToList();
                ct.HT_Quyen_NhomDTO = HT_QN;
                if (hT_NhomNguoiDung == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        //getlist ID_DonVi đc xem các danh sách giao dịch by id_nguoidung
        public List<DM_DonViXemDanhSachDTO> GetDonViXemDsGiaoDich(string quyen, Guid idnhanvien, Guid idnguoidung)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.getDonViByQuyen(quyen, idnhanvien, idnguoidung);
            }
        }

        public IHttpActionResult GetHT_Quyen_ByNguoiDung(Guid idNguoiDung)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var data = from ht_quyen_nhom in db.HT_Quyen_Nhom
                               join ht_quyen in db.HT_Quyen on ht_quyen_nhom.MaQuyen equals ht_quyen.MaQuyen
                               join ht_nguoidung_nhom in db.HT_NguoiDung_Nhom on ht_quyen_nhom.ID_NhomNguoiDung equals ht_nguoidung_nhom.IDNhomNguoiDung
                               where ht_nguoidung_nhom.IDNguoiDung == idNguoiDung
                               select new
                               {
                                   ht_quyen_nhom.MaQuyen,
                                   ht_quyen.DuocSuDung
                               };

                    if (data == null || data.Count() == 0)
                    {
                        return NotFound();
                    }
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("HT_NguoiDungAPI_GetHT_Quyen_ByNguoiDung: " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        public List<Bang_QuyenDTO> GetAllQuyen()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.getallQuyen().ToList();
            }
        }

        [HttpGet]
        public bool Check_ID_NhanVienExist(Guid idNhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.Check_ID_NhanVienExist(idNhanVien);
            }
        }

        [HttpGet]
        public bool Check_ID_NhanVienEditExist(Guid idNhanVien, Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.Check_ID_NhanVienEditExist(idNhanVien, id);
            }
        }

        [HttpGet]
        public bool Check_TenTaiKhoanExist(string tenTaiKhoan, Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.Check_TenTaiKhoanExist(tenTaiKhoan, id);
            }
        }

        [HttpGet]
        public bool Check_TenTaiKhoanAddExist(string tenTaiKhoan)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.Check_TenTaiKhoanAddExist(tenTaiKhoan);
            }
        }
        [HttpPost]
        public string CheckMatKhauKhiXoaDL(user model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                HT_NguoiDung ht = _classND.Select_NguoiDung(model.tenTaiKhoan, model.pass, "");
                if (ht != null)
                {
                    return "";
                }
                else
                {
                    return "Lỗi";
                }
            }
        }
        //loadthongbao
        [HttpGet]
        public string UpdateNguoiDocTB(Guid idtb, string nguoidoc)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                HT_ThongBao httb = _classND.GetThongBao(p => p.ID == idtb);
                string lstnguoidoc = httb.NguoiDungDaDoc;
                List<string> lstIDNHH = new List<string>();
                if (lstnguoidoc != null)
                {
                    var arrIDNHH = lstnguoidoc.Split(',');
                    for (int i = 0; i < arrIDNHH.Length; i++)
                    {
                        lstIDNHH.Add(arrIDNHH[i]);
                    }
                }
                httb.NguoiDungDaDoc = httb.NguoiDungDaDoc != "" ? (lstIDNHH.Contains(nguoidoc) ? httb.NguoiDungDaDoc : httb.NguoiDungDaDoc + "," + nguoidoc) : nguoidoc;
                _classND.Update_ThongBao(httb);
                if (httb != null)
                {
                    return "";
                }
                else
                {
                    return "Lỗi";
                }
            }
        }

        [ResponseType(typeof(HT_ThongBao_CaiDatDTO))]
        public IHttpActionResult GetCaiDatThongBao(Guid id_nguoidung)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HT_ThongBao_CaiDat httbcd = db.HT_ThongBao_CaiDat.Where(p => p.ID_NguoiDung == id_nguoidung).FirstOrDefault();
                HT_ThongBao_CaiDatDTO tbcd = new HT_ThongBao_CaiDatDTO();
                if (httbcd == null)
                {
                    return NotFound();
                }
                else
                {
                    tbcd.ID = httbcd.ID;
                    tbcd.NhacSinhNhat = httbcd.NhacSinhNhat;
                    tbcd.NhacDieuChuyen = httbcd.NhacDieuChuyen;
                    tbcd.NhacTonKho = httbcd.NhacTonKho;
                    tbcd.NhacLoHang = httbcd.NhacLoHang.Value;
                    tbcd.ID_NguoiDung = httbcd.ID_NguoiDung;
                    tbcd.BaoDuongXe = httbcd.BaoDuongXe == null ? 0 : httbcd.BaoDuongXe.Value;
                }
                return Ok(tbcd);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult UpdateCaiDatThongBao([FromBody] JObject objIn)
        {
            try
            {
                bool NhacSinhNhat = false;
                bool NhacTonKho = false;
                bool NhacDieuChuyen = false;
                bool NhacLoHang = false;
                bool NhacBaoDuong = false;
                if (objIn["NhacSinhNhat"] != null)
                    NhacSinhNhat = objIn["NhacSinhNhat"].ToObject<bool>();
                if (objIn["NhacTonKho"] != null)
                    NhacTonKho = objIn["NhacTonKho"].ToObject<bool>();
                if (objIn["NhacDieuChuyen"] != null)
                    NhacDieuChuyen = objIn["NhacDieuChuyen"].ToObject<bool>();
                if(objIn["NhacLoHang"] != null)
                    NhacLoHang = objIn["NhacLoHang"].ToObject<bool>();
                if (objIn["NhacBaoDuong"] != null)
                    NhacBaoDuong = objIn["NhacBaoDuong"].ToObject<bool>();
                string sIdNguoiDung ="";
                if (objIn["IdNguoiDung"] != null)
                    sIdNguoiDung = objIn["IdNguoiDung"].ToObject<string>();

                Guid IdNguoiDung = Guid.Empty;
                if (sIdNguoiDung != "")
                    IdNguoiDung = new Guid(sIdNguoiDung);

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    HT_ThongBao_CaiDat httbcd = db.HT_ThongBao_CaiDat.Where(p => p.ID_NguoiDung == IdNguoiDung).FirstOrDefault();
                    if (httbcd != null)
                    {
                        httbcd.NhacSinhNhat = NhacSinhNhat;
                        httbcd.NhacTonKho = NhacTonKho;
                        httbcd.NhacDieuChuyen = NhacDieuChuyen;
                        httbcd.NhacLoHang = NhacLoHang;
                        httbcd.BaoDuongXe = NhacBaoDuong ? 1 : 0;
                        db.Entry(httbcd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        HT_ThongBao_CaiDat tbcd = new HT_ThongBao_CaiDat();
                        tbcd.ID = Guid.NewGuid();
                        tbcd.NhacSinhNhat = NhacSinhNhat;
                        tbcd.NhacTonKho = NhacTonKho;
                        tbcd.NhacDieuChuyen = NhacDieuChuyen;
                        tbcd.NhacLoHang = NhacLoHang;
                        tbcd.BaoDuongXe = NhacBaoDuong ? 1 : 0;
                        tbcd.ID_NguoiDung = IdNguoiDung;
                        db.HT_ThongBao_CaiDat.Add(tbcd);
                        db.SaveChanges();
                    }
                }
                return ActionTrueNotData("");
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetThongBao([FromBody] JObject objIn)
        {
            try
            {
                bool NhacSinhNhat = false;
                bool NhacTonKho = false;
                bool NhacDieuChuyen = false;
                bool NhacLoHang = false;
                bool NhacBaoDuong = false;
                if (objIn["NhacSinhNhat"] != null)
                    NhacSinhNhat = objIn["NhacSinhNhat"].ToObject<bool>();
                if (objIn["NhacTonKho"] != null)
                    NhacTonKho = objIn["NhacTonKho"].ToObject<bool>();
                if (objIn["NhacDieuChuyen"] != null)
                    NhacDieuChuyen = objIn["NhacDieuChuyen"].ToObject<bool>();
                if (objIn["NhacLoHang"] != null)
                    NhacLoHang = objIn["NhacLoHang"].ToObject<bool>();
                if (objIn["NhacBaoDuong"] != null)
                    NhacBaoDuong = objIn["NhacBaoDuong"].ToObject<bool>();
                string sIdNguoiDung = "";
                if (objIn["IdNguoiDung"] != null)
                    sIdNguoiDung = objIn["IdNguoiDung"].ToObject<string>();

                Guid IdNguoiDung = Guid.Empty;
                if (sIdNguoiDung != "")
                    IdNguoiDung = new Guid(sIdNguoiDung);
                int CurrentPage = 0;
                if (objIn["CurrentPage"] != null)
                    CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                int PageSize = 10;
                if (objIn["PageSize"] != null)
                    PageSize = objIn["PageSize"].ToObject<int>();

                string sIdDonVi = "";
                if (objIn["IdDonVi"] != null)
                    sIdDonVi = objIn["IdDonVi"].ToObject<string>();

                Guid IdDonVi = Guid.Empty;
                if (sIdDonVi != "")
                    IdDonVi = new Guid(sIdDonVi);
                List<ObjectHTThongBao> lstTB = new List<ObjectHTThongBao>();
                int CountChuaDoc = 0;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classHTThongBao cthongbao = new classHTThongBao(db);
                    List<GetListThongBao> lstThongBao = cthongbao.HTGetListThongBao(IdDonVi, sIdNguoiDung, NhacSinhNhat, NhacTonKho, NhacDieuChuyen, NhacLoHang, NhacBaoDuong, CurrentPage, PageSize);

                    if(lstThongBao.Count > 0)
                    {
                        CountChuaDoc = lstThongBao[0].ChuaDoc;
                    }    
                    foreach (var item in lstThongBao)
                    {
                        item.NoiDungThongBao = item.NoiDungThongBao.Replace("key", item.ID.ToString());
                        System.DateTime ngaytao = item.NgayTao;
                        System.DateTime now = DateTime.Now;
                        System.TimeSpan diff = now.Subtract(ngaytao);
                        string date = "";

                        double tonggio = diff.TotalHours;
                        if (1 < tonggio && tonggio < 24)
                        {
                            date = Math.Floor(tonggio) + " giờ trước";
                        }
                        if (tonggio < 1)
                        {
                            if (diff.TotalMinutes < 1)
                            {
                                date = "vài giây trước";
                            }
                            else
                            {
                                date = Math.Floor(diff.TotalMinutes) + " phút trước";
                            }
                        }
                        if (tonggio > 24 && (item.NgayTao.Day + 1) == now.Day)
                        {
                            date = "Hôm qua lúc " + item.NgayTao.ToString("HH:ss");
                        }
                        if (tonggio > 24 && (item.NgayTao.Day + 1) != now.Day)
                        {
                            date = item.NgayTao.Day + " Tháng " + item.NgayTao.Month + " lúc " + item.NgayTao.ToString("HH:ss");
                        }
                        ObjectHTThongBao thongbao = new ObjectHTThongBao
                        {
                            NoiDungThongBao = item.NoiDungThongBao.ToString(),
                            NgayTao = date,
                            DaDoc = item.NguoiDungDaDoc != "" ? (item.NguoiDungDaDoc.Contains(IdNguoiDung.ToString()) ? true : false) : false,
                            Image = item.LoaiThongBao == 3 ? "<img src=\"/Content/images/anhhh/gato.png\" height=\"30\"/>" : (item.LoaiThongBao == 1 ? "<img src=\"/Content/images/anhhh/trao.png\" height=\"30\"/>" : "<img src=\"/Content/images/anhhh/hetkho.png\" height=\"30\"/>"),
                        };
                        lstTB.Add(thongbao);
                    }
                }
                return ActionTrueData(new { 
                    ListThongBao = lstTB,
                    CountTB = CountChuaDoc
                });
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        [HttpGet]
        public string UpdateDaDocAll(string nguoidoc, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                List<HT_ThongBao> httb = _classND.GetThongBaos(p => p.ID_DonVi == iddonvi && !p.NguoiDungDaDoc.Contains(nguoidoc)).ToList();
                foreach (var item in httb)
                {
                    string lstnguoidoc = item.NguoiDungDaDoc;
                    List<string> lstIDNHH = new List<string>();
                    if (lstnguoidoc != null)
                    {
                        var arrIDNHH = lstnguoidoc.Split(',');
                        for (int i = 0; i < arrIDNHH.Length; i++)
                        {
                            lstIDNHH.Add(arrIDNHH[i]);
                        }
                    }
                    item.NguoiDungDaDoc = item.NguoiDungDaDoc != "" ? (lstIDNHH.Contains(nguoidoc) ? item.NguoiDungDaDoc : item.NguoiDungDaDoc + "," + nguoidoc) : nguoidoc;
                    _classND.Update_ThongBao(item);
                }
                return "";
            }
        }
        #endregion

        #region insert
        [HttpPost, ActionName("PostHT_NguoiDung")]
        [ResponseType(typeof(HT_NguoiDung))]
        public IHttpActionResult PostHT_NguoiDung([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                        ClassNS_NhanVien _classNV = new ClassNS_NhanVien(db);
                        classHT_NguoiDung_Nhom _classNDN = new classHT_NguoiDung_Nhom(db);
                        HT_NguoiDung objNewVT = data["objNewND"].ToObject<HT_NguoiDung>();
                        Guid id = data["id"].ToObject<Guid>();
                        string TenTaiKhoan = objNewVT.TaiKhoan;

                        #region HT_NguoiDung
                        HT_NguoiDung hT_NguoiDung = new HT_NguoiDung();
                        hT_NguoiDung.ID = Guid.NewGuid();
                        hT_NguoiDung.ID_NhanVien = objNewVT.ID_NhanVien;
                        hT_NguoiDung.LaAdmin = false;
                        hT_NguoiDung.LaNhanVien = true;
                        hT_NguoiDung.DangHoatDong = objNewVT.DangHoatDong;
                        hT_NguoiDung.TaiKhoan = objNewVT.TaiKhoan;
                        hT_NguoiDung.MatKhau = objNewVT.MatKhau;
                        hT_NguoiDung.NgayTao = DateTime.Now;
                        hT_NguoiDung.NguoiTao = objNewVT.NguoiTao;
                        hT_NguoiDung.ID_DonVi = objNewVT.ID_DonVi;
                        #endregion

                        string strIns = _classND.PostHT_NguoiDung(hT_NguoiDung).Result;
                        HT_NguoiDung_Nhom hT_NguoiDung_Nhom = new HT_NguoiDung_Nhom();
                        hT_NguoiDung_Nhom.ID = Guid.NewGuid();
                        hT_NguoiDung_Nhom.IDNguoiDung = hT_NguoiDung.ID;
                        hT_NguoiDung_Nhom.IDNhomNguoiDung = id;
                        hT_NguoiDung_Nhom.ID_DonVi = objNewVT.ID_DonVi;
                        _classNDN.Add_NguoiDung_Nhom(hT_NguoiDung_Nhom);

                        HT_ThongBao_CaiDat thongbaocd = new HT_ThongBao_CaiDat();
                        thongbaocd.ID = Guid.NewGuid();
                        thongbaocd.ID_NguoiDung = hT_NguoiDung.ID;
                        thongbaocd.NhacCongNo = true;
                        thongbaocd.NhacDieuChuyen = true;
                        thongbaocd.NhacSinhNhat = true;
                        thongbaocd.NhacTonKho = true;
                        db.HT_ThongBao_CaiDat.Add(thongbaocd);
                        db.SaveChanges();
                        trans.Commit();

                        HT_NguoiDungDTO objReturn = new HT_NguoiDungDTO();
                        objReturn.ID = hT_NguoiDung.ID;
                        objReturn.ID_NhanVien = hT_NguoiDung.ID_NhanVien;
                        objReturn.ID_DonVi = hT_NguoiDung.ID_DonVi;
                        objReturn.LaAdmin = hT_NguoiDung.LaAdmin;
                        objReturn.DangHoatDong = hT_NguoiDung.DangHoatDong;
                        objReturn.TaiKhoan = hT_NguoiDung.TaiKhoan;
                        objReturn.MatKhau = hT_NguoiDung.MatKhau;
                        return Json(new { res = true, data = objReturn });
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Json(new { res = false, mes = e.InnerException + e.Message });
                    }
                }
            }
        }

        [HttpPost, ActionName("PostHT_NhomNguoiDung")]
        [ResponseType(typeof(HT_NhomNguoiDung))]
        public IHttpActionResult PostHT_NhomNguoiDung([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                //DM_ViTri objNewVT = data.ToObject<DM_ViTri>();
                HT_NhomNguoiDung objVaiTro = data["objVaiTro"].ToObject<HT_NhomNguoiDung>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                List<string> objQuyenNhom = new List<string>();
                if (data["objQuyenNhom"] != null)
                {
                    objQuyenNhom = data["objQuyenNhom"].ToObject<List<string>>();
                }
                string tenNhom = objVaiTro.TenNhom;


                #region HT_NguoiDung
                HT_NhomNguoiDung HT_NhomNguoiDung = new HT_NhomNguoiDung();
                HT_NhomNguoiDung.ID = Guid.NewGuid();
                HT_NhomNguoiDung.MaNhom = "NND001";
                HT_NhomNguoiDung.TenNhom = objVaiTro.TenNhom;
                string strIns = _classNND.Add_NhomNguoiDung(HT_NhomNguoiDung);

                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                hT_NhatKySuDung.ChucNang = "Quản lý người dùng";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Thêm mới vai trò: " + objVaiTro.TenNhom;
                hT_NhatKySuDung.NoiDungChiTiet = "Thêm mới vai trò: " + objVaiTro.TenNhom;
                hT_NhatKySuDung.LoaiNhatKy = 1;
                hT_NhatKySuDung.ID_DonVi = iddonvi;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                HT_Quyen_Nhom ht_QuyenNhom = new HT_Quyen_Nhom();
                objQuyenNhom = _classNND.GetListQuyenChaFromListQuyen(objQuyenNhom);
                var listInsert = objQuyenNhom.Select(p => new HT_Quyen_Nhom
                {
                    ID = Guid.NewGuid(),
                    ID_NhomNguoiDung = HT_NhomNguoiDung.ID,
                    MaQuyen = p
                }).AsEnumerable();

                _classNND.AddQuyenNhomNew(listInsert);

                #endregion
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    HT_NhomNguoiDungDTO objReturn = new HT_NhomNguoiDungDTO();
                    objReturn.ID = HT_NhomNguoiDung.ID;
                    objReturn.MaNhom = HT_NhomNguoiDung.MaNhom;
                    objReturn.TenNhom = HT_NhomNguoiDung.TenNhom;
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }
        //get nhóm người dùng
        [ResponseType(typeof(HT_NhomNguoiDung))]
        public IHttpActionResult Get_NhomNguoiDung(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                HT_NhomNguoiDung hT_NhomNguoiDung = _classNND.Select_NhomNguoiDung(id);
                HT_NhomNguoiDung temp = new HT_NhomNguoiDung();
                temp.ID = hT_NhomNguoiDung.ID;
                temp.TenNhom = hT_NhomNguoiDung.TenNhom;
                if (hT_NhomNguoiDung == null)
                {
                    return NotFound();
                }
                return Ok(temp);
            }
        }
        #endregion

        #region update
        public bool Check_LoGin(string tenTaiKhoan, string matKhau)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                bool exUser = _classND.Check_LoGin(tenTaiKhoan, matKhau);
                return exUser;
            }
        }

        public bool CheckAdmin(Guid idnd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                if (db == null)
                {
                    return false;
                }
                else
                {
                    HT_NguoiDung htnd = _classND.Get(p => p.ID == idnd);
                    if (htnd.LaAdmin == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // PUT: api/DM_ViTriAPI/5

        [HttpPost]
        public IHttpActionResult PutHT_NguoiDung([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                ClassNS_NhanVien _classNV = new ClassNS_NhanVien(db);
                Guid id = data["id"].ToObject<Guid>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                HT_NguoiDung hT_NguoiDung = data["objNewND"].ToObject<HT_NguoiDung>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = _classND.Put_NguoiDung(hT_NguoiDung).Result;
                //add lich sử thao tác
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                hT_NhatKySuDung.ChucNang = "Quản lý người dùng";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Cập nhật người dùng tên: " + _classNV.Get(p => p.ID == hT_NguoiDung.ID_NhanVien).TenNhanVien + ", tên đăng nhập: " + hT_NguoiDung.TaiKhoan;
                hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật người dùng tên: " + _classNV.Get(p => p.ID == hT_NguoiDung.ID_NhanVien).TenNhanVien + ", tên đăng nhập: " + hT_NguoiDung.TaiKhoan;
                hT_NhatKySuDung.LoaiNhatKy = 2;
                hT_NhatKySuDung.ID_DonVi = hT_NguoiDung.ID_DonVi.Value;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    HT_NguoiDungDTO objReturn = new HT_NguoiDungDTO();
                    objReturn.ID = hT_NguoiDung.ID;
                    objReturn.ID_NhanVien = hT_NguoiDung.ID_NhanVien;
                    objReturn.ID_DonVi = hT_NguoiDung.ID_DonVi;
                    objReturn.LaAdmin = hT_NguoiDung.LaAdmin;
                    objReturn.DangHoatDong = hT_NguoiDung.DangHoatDong;
                    objReturn.TaiKhoan = hT_NguoiDung.TaiKhoan;
                    objReturn.MatKhau = hT_NguoiDung.MatKhau;
                    var cookie = HttpContext.Current.Request.Cookies.Get("Account");
                    var json = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(cookie.Value), "SSOFTVN");
                    var ison2 = json.Replace("%0d%0a", "\r\n");
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var result = serializer.Deserialize<HT_NguoiDung>(ison2);
                    if (id == result.ID)
                    {
                        dynamic user = new
                        {
                            MatKhau = hT_NguoiDung.MatKhau == "" ? result.MatKhau : hT_NguoiDung.MatKhau,
                            TaiKhoan = hT_NguoiDung.TaiKhoan,
                            ID_NhanVien = hT_NguoiDung.ID_NhanVien,
                            ID = hT_NguoiDung.ID,
                            ID_DonVi = hT_NguoiDung.ID_DonVi,
                            LaAdmin = hT_NguoiDung.LaAdmin,
                        };
                        var json1 = JsonConvert.SerializeObject(user);
                        string jsonconvert = Convert.ToBase64String(Model.AesEncrypt.EncryptStringToBytes_Aes(json1, "SSOFTVN"));
                        var response = HttpContext.Current.Response;
                        response.Cookies.Remove("Account");
                        //1 thang xoa cookies 1 lan
                        cookie.Expires = DateTime.Now.AddMonths(1);
                        cookie.Value = jsonconvert;
                        response.Cookies.Add(cookie);
                    }
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }


        [ResponseType(typeof(string))]
        public IHttpActionResult PutHT_NguoiDungXemGiaVon([FromBody] JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            HT_NguoiDung hT_NguoiDung = data["objNewND"].ToObject<HT_NguoiDung>();
            //DM_ViTri.ID = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            HT_NguoiDung objUpd = db.HT_NguoiDung.Find(id);
            objUpd.XemGiaVon = hT_NguoiDung.XemGiaVon;
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        public void Put_HT_NguoiDung_Nhom([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung_Nhom _classNDN = new classHT_NguoiDung_Nhom(db);
                HT_DonVi_VaiTro objVaiTro = data["objVaiTro"].ToObject<HT_DonVi_VaiTro>();
                if (objVaiTro.ID == Guid.Empty)
                {
                    HT_NguoiDung_Nhom htndnhom = new HT_NguoiDung_Nhom();
                    htndnhom.ID = Guid.NewGuid();
                    htndnhom.IDNhomNguoiDung = objVaiTro.ID_VaiTro;
                    htndnhom.IDNguoiDung = objVaiTro.IDNguoiDung;
                    htndnhom.ID_DonVi = objVaiTro.ID_DonVi;
                    _classNDN.Add_NguoiDung_Nhom(htndnhom);
                }
                else
                {
                    HT_NguoiDung_Nhom htnhomnd = db.HT_NguoiDung_Nhom.Find(objVaiTro.ID);
                    if (objVaiTro.ID_VaiTro == Guid.Empty)
                    {
                        db.HT_NguoiDung_Nhom.Remove(htnhomnd);
                        db.SaveChanges();
                    }
                    else
                    {
                        htnhomnd.IDNhomNguoiDung = objVaiTro.ID_VaiTro;
                        _classNDN.Update_NhomNguoiDung(htnhomnd);

                    }
                }
            }
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult PutHTVaiTro([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                HT_NhomNguoiDung objVaiTro = data["objVaiTro"].ToObject<HT_NhomNguoiDung>();
                Guid id = data["idnhom"].ToObject<Guid>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                List<string> objQuyenNhom = new List<string>();
                if (data["objQuyenNhom"] != null)
                {
                    objQuyenNhom = data["objQuyenNhom"].ToObject<List<string>>();
                }
                //var con = objQuyenNhom.Where(o => o.Split(',').Length == 3).Select(o => o.Split(',')[0]).ToList();
                //var cha = objQuyenNhom.Where(o => o.Split(',').Length == 3).Select(o => o.Split(',')[1]).Distinct().ToList();
                //cha.AddRange(objQuyenNhom.Where(o => o.Split(',').Length == 2).Select(o => o.Split(',')[0]).Distinct().ToList());
                //var ong = objQuyenNhom.Where(o => o.Split(',').Length == 3).Select(o => o.Split(',')[2]).Distinct().ToList();
                //ong.AddRange(objQuyenNhom.Where(o => o.Split(',').Length == 2).Select(o => o.Split(',')[1]).Distinct().ToList());

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                HT_NhomNguoiDung ht_nhomnd = _classNND.Select_NhomNguoiDung(id);
                ht_nhomnd.TenNhom = objVaiTro.TenNhom;
                string strDel = _classNND.Update_NhomNguoiDung(ht_nhomnd);
                //add lịch sử hđ
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                hT_NhatKySuDung.ChucNang = "Quản lý người dùng";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Cập nhật vai trò: " + objVaiTro.TenNhom;
                hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật vai trò: " + objVaiTro.TenNhom;
                hT_NhatKySuDung.LoaiNhatKy = 2;
                hT_NhatKySuDung.ID_DonVi = iddonvi;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                //Quyen
                //con.AddRange(ong);
                //con.AddRange(cha);
                objQuyenNhom = _classNND.GetListQuyenChaFromListQuyen(objQuyenNhom);
                List<string> quyenold = db.HT_Quyen_Nhom.Where(p => p.ID_NhomNguoiDung == ht_nhomnd.ID).Select(p => p.MaQuyen).ToList();
                List<string> quyenadd = objQuyenNhom.Except(quyenold).ToList();
                List<string> quyenRemove = quyenold.Except(objQuyenNhom).ToList();
                db.HT_Quyen_Nhom.RemoveRange(db.HT_Quyen_Nhom.Where(idDV => idDV.ID_NhomNguoiDung == ht_nhomnd.ID).Where(p => quyenRemove.Contains(p.MaQuyen)));
                db.SaveChanges();

                //var listInsert = quyenadd.Distinct().Select(p => new HT_Quyen_Nhom
                //{
                //    ID = Guid.NewGuid(),
                //    ID_NhomNguoiDung = ht_nhomnd.ID,
                //    MaQuyen = p
                //}).AsEnumerable();
                //db.HT_Quyen_Nhom.AddRange(listInsert);
                //db.SaveChanges();
                foreach (var item in quyenadd)
                {
                    HT_Quyen_Nhom ht_QuyenNhom = new HT_Quyen_Nhom();
                    ht_QuyenNhom.ID = Guid.NewGuid();
                    ht_QuyenNhom.ID_NhomNguoiDung = ht_nhomnd.ID;
                    ht_QuyenNhom.MaQuyen = item.ToString();
                    _classNND.Add_QuyenNhom(ht_QuyenNhom);
                }
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion

        #region delete
        // DELETE: api/DM_HT_NguoiDungAPI/5
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string DeleteHT_NguoiDung(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classND = new classHT_NguoiDung(db);
                return _classND.DeleteHT_NguoiDung(id);
            }
        }

        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult DeleteHT_NhomNguoiDung(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
                string strDel = _classNND.Delete_NhomNguoiDung(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        private class HT_ThongBao_CaiDatDTO
        {
            public Guid ID { get; set; }
            public Guid ID_NguoiDung { get; set; }
            public bool NhacSinhNhat { get; set; }
            public bool NhacCongNo { get; set; }
            public bool NhacTonKho { get; set; }
            public bool NhacDieuChuyen { get; set; }
            public bool NhacLoHang { get; set; }
            public int BaoDuongXe { get; set; }
        }
        public class user
        {
            public string tenTaiKhoan { get; set; }
            public string pass { get; set; }
        }
        #endregion
    }
}
