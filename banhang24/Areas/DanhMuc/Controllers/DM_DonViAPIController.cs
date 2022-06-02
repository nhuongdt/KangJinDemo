using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using libDM_DonVi;
using Model;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using libDM_Kho;
using libHT;
using System.Data.SqlClient;
using libNS_NhanVien;
using libHT_NguoiDung;
using Model_banhang24vn.DAL;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_DonViAPIController : ApiController
    {
        #region GET
        // GET: api/DanhMuc/DM_ViTriAPI/GetDM_ViTri
        public IQueryable<DM_DonVi> GetDM_DonVi()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                return _classDMDV.Gets(null);
            }
        }

        [HttpGet]
        public string GetMaChiNhanh_byID(Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi classHTCongTy = new classDM_DonVi(db);
                return classHTCongTy.Get(x => x.ID == idDonVi).MaDonVi;
            }
        }

        [HttpGet]
        public bool Check_MaChiNhanhEmpty()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi classHTCongTy = new classDM_DonVi(db);
                var data = classHTCongTy.Gets(x => x.MaDonVi == string.Empty || x.MaDonVi == null).ToList();
                if (data.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public List<DM_DonViDTO> GetListDonVi(string hoatdong, string txtCN)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                IQueryable<DM_DonVi> lstAllVTs = _classDMDV.Gets(null);
                if (hoatdong == "1")
                {
                    lstAllVTs = lstAllVTs.Where(p => p.TrangThai == true || p.TrangThai == null);
                }
                else
                {
                    if (hoatdong == "2")
                    {
                        lstAllVTs = lstAllVTs.Where(p => p.TrangThai == false);
                    }
                }
                if (lstAllVTs != null && lstAllVTs.Count() > 0)
                {
                    List<DM_DonViDTO> lsrReturns = new List<DM_DonViDTO>();
                    foreach (DM_DonVi item in lstAllVTs)
                    {
                        DM_DonViDTO itemData = new DM_DonViDTO
                        {
                            ID = item.ID,
                            MaDonVi = item.MaDonVi,
                            TenDonVi = item.TenDonVi,
                            DiaChi = item.DiaChi,
                            SoDienThoai = item.SoDienThoai,
                            TrangThai = item.TrangThai,
                            TenDonViBoDAu = CommonStatic.ConvertToUnSign(item.TenDonVi).ToLower(),
                            TenDonViKTD = CommonStatic.GetCharsStart(item.TenDonVi).ToLower(),
                        };
                        lsrReturns.Add(itemData);
                    }
                    var search = CommonStatic.ConvertToUnSign(txtCN).ToLower();
                    lsrReturns = lsrReturns.Where(x => x.TenDonVi.Contains(@search) || x.TenDonViBoDAu.Contains(@search) || x.TenDonViKTD.Contains(@search)).ToList();
                    return lsrReturns.OrderBy(p => p.TenDonVi).ToList();
                }
                else
                    return null;
            }
        }

        [banhang24.Compress.DeflateCompression]
        public List<DM_DonViDTO> GetListDonVi1()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                IQueryable<DM_DonVi> lstAllVTs = _classDMDV.Gets(p => p.TrangThai == true || p.TrangThai == null);
                if (lstAllVTs != null && lstAllVTs.Count() > 0)
                {
                    List<DM_DonViDTO> lsrReturns = new List<DM_DonViDTO>();
                    foreach (DM_DonVi item in lstAllVTs)
                    {
                        DM_DonViDTO itemData = new DM_DonViDTO
                        {
                            ID = item.ID,
                            TenDonVi = item.TenDonVi,
                            DiaChi = item.DiaChi,
                            SoDienThoai = item.SoDienThoai,
                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns.OrderBy(o => o.TenDonVi).ToList();
                }
                else
                    return null;
            }
        }

        public List<DM_DonViDTO> GetListDonVi(string id)
        {
            using (SsoftvnContext db = new SsoftvnContext(id))
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                IQueryable<DM_DonVi> lstAllVTs = _classDMDV.Gets(p => p.TrangThai == true || p.TrangThai == null);
                if (lstAllVTs != null && lstAllVTs.Count() > 0)
                {
                    List<DM_DonViDTO> lsrReturns = new List<DM_DonViDTO>();
                    foreach (DM_DonVi item in lstAllVTs)
                    {
                        DM_DonViDTO itemData = new DM_DonViDTO
                        {
                            ID = item.ID,
                            TenDonVi = item.TenDonVi,
                            DiaChi = item.DiaChi,
                            SoDienThoai = item.SoDienThoai,
                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns.OrderBy(o => o.TenDonVi).ToList();
                }
                else
                    return null;
            }
        }

        // not order by because order at .js
        public IHttpActionResult GetListDonVi_nhuongdt()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                try
                {
                    var lst = _classDMDV.Gets(p => p.TrangThai == true || p.TrangThai == null).
                    Select(x => new { x.ID, x.TenDonVi, x.DiaChi, x.SoDienThoai });
                    return Json(new { res = true, data = lst });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }
        [HttpGet]
        public List<DM_DonVi_ChotSo> ChotSo_GetListDonVi()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDM_DonVi _classDMDV = new classDM_DonVi(db);
                    List<DM_DonVi_ChotSo> lst = _classDMDV.ChotSo_GetListDonVi();
                    return lst;
                }
                catch (Exception)
                {
                    return new List<DM_DonVi_ChotSo>();
                }
            }
        }
        public List<DM_DonViDTO> GetListDonViByIDNguoiDung(Guid? idnhanvien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                return _classDMDV.getListDVByIDNguoiDung(idnhanvien);
            }
        }

        public List<HT_DonVi_VaiTro> getListIDDonViVaiTro(Guid? idnhanvien, Guid idnguoidung)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                return _classDMDV.getListIDDonViVaiTro(idnhanvien, idnguoidung);
            }
        }

        public List<DM_DonViDTO> GetListDonViByID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                List<DM_DonViDTO> dm_donvi = _classDMDV.getListDVByID(id);
                return dm_donvi;
            }
        }
        [HttpGet]
        public List<DM_DonVi_byUser> GetDonVi_byUser(Guid ID_NguoiDung, string TenDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<DM_DonVi_byUser> lst = new List<DM_DonVi_byUser>();
                string _tenDonVi = string.Empty;
                if (TenDonVi != "" & TenDonVi != null & TenDonVi != "null")
                    _tenDonVi = "%" + TenDonVi + "%";
                else
                    _tenDonVi = "%%";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
                prm.Add(new SqlParameter("TenDonVi", _tenDonVi));
                lst = db.Database.SqlQuery<DM_DonVi_byUser>("exec GetDonVi_byUser @ID_NguoiDung, @TenDonVi ", prm.ToArray()).ToList();
                return lst;
            }
        }
        [HttpGet]
        public List<DM_DonVi_byUser> GetDonVi_byUserSearch(Guid ID_NguoiDung, string TenDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                List<DM_DonVi_byUser> lst = new List<DM_DonVi_byUser>();
                string _tenDonVi = string.Empty;
                if (TenDonVi != "" & TenDonVi != null & TenDonVi != "null")
                    _tenDonVi = "%" + TenDonVi + "%";
                else
                    _tenDonVi = "%%";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
                prm.Add(new SqlParameter("TenDonVi", _tenDonVi));
                HT_NguoiDung ngdung = _classHTND.Get(p => p.ID == ID_NguoiDung);
                if (ngdung.LaAdmin == false)
                {
                    lst = db.Database.SqlQuery<DM_DonVi_byUser>("exec GetDonVi_byUserSeach @ID_NguoiDung, @TenDonVi ", prm.ToArray()).ToList();
                }
                else
                {
                    lst = db.Database.SqlQuery<DM_DonVi_byUser>("exec GetDonVi_byUser @ID_NguoiDung, @TenDonVi ", prm.ToArray()).ToList();
                }
                return lst;
            }
        }
        public List<DM_DonViDTO> GetListDonVi_User(Guid ID_NguoiDung)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                List<DM_DonVi> lstAllVTs = _classDMDV.GetDonVi_User(ID_NguoiDung);
                if (lstAllVTs != null && lstAllVTs.Count() > 0)
                {
                    List<DM_DonViDTO> lsrReturns = new List<DM_DonViDTO>();
                    foreach (DM_DonVi item in lstAllVTs)
                    {
                        DM_DonViDTO itemData = new DM_DonViDTO
                        {
                            ID = item.ID,
                            TenDonVi = item.TenDonVi,
                            DiaChi = item.DiaChi,
                            SoDienThoai = item.SoDienThoai,
                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                else
                    return null;
            }
        }
        public List<DM_DonViDTO> GetListDonVi_User()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var ID_NguoiDung = new Guid(CookieStore.GetCookieAes(Hellper.SystemConsts.NGUOIDUNGID));
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                List<DM_DonVi> lstAllVTs = _classDMDV.GetDonVi_User(ID_NguoiDung);
                if (lstAllVTs != null && lstAllVTs.Count() > 0)
                {
                    List<DM_DonViDTO> lsrReturns = new List<DM_DonViDTO>();
                    foreach (DM_DonVi item in lstAllVTs)
                    {
                        DM_DonViDTO itemData = new DM_DonViDTO
                        {
                            ID = item.ID,
                            MaDonVi = item.MaDonVi,
                            TenDonVi = item.TenDonVi,
                            DiaChi = item.DiaChi,
                            SoDienThoai = item.SoDienThoai,
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
        [ResponseType(typeof(DM_DonViDTO))]
        public IHttpActionResult GetDM_DonVi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                DM_DonVi DM_DonVi = _classDMDV.Select_DonVi(id);
                DM_DonViDTO ct = new DM_DonViDTO();
                ct.ID = DM_DonVi.ID;
                ct.TenDonVi = DM_DonVi.TenDonVi;
                ct.DiaChi = DM_DonVi.DiaChi;
                ct.SoDienThoai = DM_DonVi.SoDienThoai;

                if (DM_DonVi == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        #endregion

        #region update
        // PUT: api/DM_DonViAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_DonVi(Guid id, DM_DonVi DM_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                if (!ModelState.IsValid)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                string strUpd = _classDMDV.Update_DonVi(DM_DonVi);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/DM_DonViAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_DonVi([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                Guid id = data["id"].ToObject<Guid>();
                DM_DonVi DM_DonVi = data["objNewDonVi"].ToObject<DM_DonVi>();

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = _classDMDV.Update_DonVi(DM_DonVi);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    DM_DonViDTO objReturn = new DM_DonViDTO();
                    objReturn.ID = DM_DonVi.ID;
                    objReturn.MaDonVi = DM_DonVi.MaDonVi;
                    objReturn.TenDonVi = DM_DonVi.TenDonVi;
                    objReturn.DiaChi = DM_DonVi.DiaChi;
                    objReturn.SoDienThoai = DM_DonVi.SoDienThoai;
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }
        #endregion

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool GioiHanSoChiNhanh()
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            int gioihan = CuaHangDangKyService.GetGioiHanChiNhanh(str);
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<DM_DonVi> lst = db.DM_DonVi.Where(x => x.TrangThai != false).ToList();
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

        #region insert
        // POST: api/DM_DonViAPI
        [HttpPost, ActionName("PostDM_DonVi1")]
        [ResponseType(typeof(DM_DonVi))]
        public IHttpActionResult PostDM_DonVi1(DM_DonVi dM_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                if (!ModelState.IsValid)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                dM_DonVi.ID = Guid.NewGuid();

                string strIns = _classDMDV.Add_DonVi(dM_DonVi);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = dM_DonVi.ID }, dM_DonVi);
            }
        }

        [HttpPost, ActionName("PostDM_DonVi")]
        [ResponseType(typeof(DM_DonVi))]
        public IHttpActionResult PostDM_DonVi([FromBody]JObject data, Guid idnhanvien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                classHT_CauHinhPhanMem _classHTCH = new classHT_CauHinhPhanMem(db);
                ClassNS_NhanVien _classNV = new ClassNS_NhanVien(db);
                classDM_Kho _classDMKho = new classDM_Kho(db);

                //DM_ViTri objNewVT = data.ToObject<DM_ViTri>();
                DM_DonVi objNewVT = data["objNewDonVi"].ToObject<DM_DonVi>();
                Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                #region DM_DonVi
                DM_DonVi dM_DonVi = new DM_DonVi();
                dM_DonVi.ID = Guid.NewGuid();
                dM_DonVi.TenDonVi = objNewVT.TenDonVi;
                dM_DonVi.DiaChi = objNewVT.DiaChi;
                dM_DonVi.SoDienThoai = objNewVT.SoDienThoai;
                dM_DonVi.NguoiTao = objNewVT.NguoiTao;
                dM_DonVi.NgayTao = DateTime.Now;
                //string MaDonVi = string.Empty;
                //if (dM_DonVi.MaDonVi == null || dM_DonVi.MaDonVi == "")
                //{
                //    MaDonVi = libDonViQuiDoi.classDonViQuiDoi.GetMaHangHoa();
                //}
                //else
                //{
                //    //mã hàng hóa tự động
                dM_DonVi.MaDonVi = objNewVT.MaDonVi;
                //}

                #endregion
                string strIns = _classDMDV.Add_DonVi(dM_DonVi);

                //add lich sử thao tác
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                hT_NhatKySuDung.ChucNang = "Quản lý chi nhánh";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Thêm mới chi nhánh: " + objNewVT.TenDonVi;
                hT_NhatKySuDung.NoiDungChiTiet = "Thêm mới chi nhánh: " + objNewVT.TenDonVi;
                hT_NhatKySuDung.LoaiNhatKy = 1;
                hT_NhatKySuDung.ID_DonVi = iddonvi;
                SaveDiary.add_Diary(hT_NhatKySuDung);

                DM_Kho dM_Kho = new DM_Kho();
                dM_Kho.ID = Guid.NewGuid();
                dM_Kho.NgayTao = DateTime.Now;
                dM_Kho.NguoiTao = objNewVT.NguoiTao;
                dM_Kho.MaKho = DateTime.Now.ToString("yyyymmddhhmm");
                dM_Kho.TenKho = "Kho " + objNewVT.TenDonVi;
                _classDMKho.Add_DMKho(dM_Kho);

                Kho_DonVi kho_donvi = new Kho_DonVi();
                kho_donvi.ID = Guid.NewGuid();
                kho_donvi.ID_Kho = dM_Kho.ID;
                kho_donvi.ID_DonVi = dM_DonVi.ID;
                _classDMKho.Add_Kho_DonVi(kho_donvi);

                //Add cấu hình phần mềm cho đơn vị mới
                HT_CauHinhPhanMem objch = db.HT_CauHinhPhanMem.FirstOrDefault();
                HT_CauHinhPhanMem hT_CauHinhPhanMem = new HT_CauHinhPhanMem();
                hT_CauHinhPhanMem.ID = Guid.NewGuid();
                hT_CauHinhPhanMem.ID_DonVi = dM_DonVi.ID;
                hT_CauHinhPhanMem.GiaVonTrungBinh = true;
                hT_CauHinhPhanMem.CoDonViTinh = true;
                hT_CauHinhPhanMem.DatHang = false;
                hT_CauHinhPhanMem.XuatAm = true;
                hT_CauHinhPhanMem.DatHangXuatAm = true;
                hT_CauHinhPhanMem.ThayDoiThoiGianBanHang = true;
                hT_CauHinhPhanMem.TinhNangTichDiem = false;
                hT_CauHinhPhanMem.GioiHanThoiGianTraHang = false;
                hT_CauHinhPhanMem.SanPhamCoThuocTinh = true;
                hT_CauHinhPhanMem.BanVaChuyenKhiHangDaDat = true;
                hT_CauHinhPhanMem.TinhNangSanXuatHangHoa = false;
                hT_CauHinhPhanMem.SuDungCanDienTu = false;
                hT_CauHinhPhanMem.KhoaSo = false;
                hT_CauHinhPhanMem.InBaoGiaKhiBanHang = true;
                hT_CauHinhPhanMem.QuanLyKhachHangTheoDonVi = true;
                hT_CauHinhPhanMem.SoLuongTrenChungTu = true;
                hT_CauHinhPhanMem.KhuyenMai = false;
                hT_CauHinhPhanMem.SuDungMauInMacDinh = false;
                hT_CauHinhPhanMem.ApDungGopKhuyenMai = false;
                hT_CauHinhPhanMem.LoHang = objch.LoHang;
                _classHTCH.add_ThietLap(hT_CauHinhPhanMem);

                NS_QuaTrinhCongTac qtct = new NS_QuaTrinhCongTac();
                qtct.ID = Guid.NewGuid();
                qtct.ID_NhanVien = idnhanvien;
                qtct.ID_DonVi = dM_DonVi.ID;
                qtct.NgayApDung = DateTime.Now;
                _classNV.Add_QuaTrinhCongTac(qtct);

                //Update tồn kho cho chi nhánh mới
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_ChiNhanh", dM_DonVi.ID));

                db.Database.ExecuteSqlCommand("exec UpdateTonKhoChoChiNhanhMoi @ID_ChiNhanh", paramlist.ToArray());

                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    DM_DonViDTO objReturn = new DM_DonViDTO();
                    objReturn.MaDonVi = dM_DonVi.MaDonVi;
                    objReturn.ID = dM_DonVi.ID;
                    objReturn.TenDonVi = dM_DonVi.TenDonVi;
                    objReturn.DiaChi = dM_DonVi.DiaChi;
                    objReturn.SoDienThoai = dM_DonVi.SoDienThoai;

                    return CreatedAtRoute("DefaultApi", new
                    {
                        id = objReturn.ID
                    }, objReturn);
                }
            }
        }

        [HttpDelete]
        [ResponseType(typeof(string))]
        public string NgungHoatDong(Guid id, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    DM_DonVi dv = db.DM_DonVi.Where(p => p.ID == id).FirstOrDefault();
                    if (dv != null)
                    {
                        dv.TrangThai = false;
                        _classDMDV.Update_DonVi(dv);

                        HT_NhatKySuDung nhatKySuDung = new HT_NhatKySuDung();
                        nhatKySuDung.ID = Guid.NewGuid();
                        nhatKySuDung.ID_NhanVien = idnhanvien;
                        nhatKySuDung.ChucNang = "Chi nhánh";
                        nhatKySuDung.ThoiGian = DateTime.Now;
                        nhatKySuDung.NoiDung = "Ngừnghoạt động chi nhánh" + dv.TenDonVi;
                        nhatKySuDung.NoiDungChiTiet = "Ngừnghoạt động chi nhánh" + dv.TenDonVi;
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

        [HttpDelete]
        [ResponseType(typeof(string))]
        public string ChoHoatDong(Guid id, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    DM_DonVi dv = db.DM_DonVi.Where(p => p.ID == id).FirstOrDefault();
                    if (dv != null)
                    {
                        dv.TrangThai = true;
                        _classDMDV.Update_DonVi(dv);

                        HT_NhatKySuDung nhatKySuDung = new HT_NhatKySuDung();
                        nhatKySuDung.ID = Guid.NewGuid();
                        nhatKySuDung.ID_NhanVien = idnhanvien;
                        nhatKySuDung.ChucNang = "Chi nhánh";
                        nhatKySuDung.ThoiGian = DateTime.Now;
                        nhatKySuDung.NoiDung = "Cho phép hoạt động Chi nhánh" + dv.TenDonVi;
                        nhatKySuDung.NoiDungChiTiet = "Cho phép hoạt động Chi nhánh" + dv.TenDonVi;
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

        #endregion

        #region delete
        // DELETE: api/DM_DonViAPI/5
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string DeleteDM_DonVi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                return _classDMDV.Delete_DonVi(id);
            }
        }
        #endregion

        #region ###

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion
    }


}
