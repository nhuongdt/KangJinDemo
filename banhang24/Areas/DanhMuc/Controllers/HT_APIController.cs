using banhang24.Hellper;
using HT;
using libHT;
using libHT_NguoiDung;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using ZaloConnectApp;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class HT_APIController : BaseApiController
    {
        public IHttpActionResult GetListYear()
        {
            try
            {
                List<int> listYear = new List<int>();
                int StartYear = DateTime.Now.Year;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    DM_DoiTuong dt = db.DM_DoiTuong.Where(p => p.ID == Guid.Empty).FirstOrDefault();
                    if (dt != null)
                    {
                        StartYear = dt.NgayTao.Value.Year;
                    }
                }
                int endYear = DateTime.Now.Year;
                listYear = Enumerable.Range(StartYear, endYear - StartYear + 1).OrderByDescending(p => p).ToList();
                return ActionTrueData(new
                {
                    data = listYear
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        #region Zaloconnect
        public IHttpActionResult getZaloCodeChallenge()
        {
            string result = null;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                HT_CongTy cty = _classHTCT.Gets(null).FirstOrDefault();
                CodeChallengeClass codeChallengeClass = new CodeChallengeClass();
                if (cty.ZaloCodeVerifier == null || cty.ZaloCodeVerifier == "")
                {
                    cty.ZaloCodeVerifier = codeChallengeClass.genCodeVerifier();
                    _classHTCT.UpdateHTCty(cty);
                }
                result = codeChallengeClass.genCodeChallenge(cty.ZaloCodeVerifier);
            }
            
            if(result != null)
            {
                return ActionTrueNotData(result);
            }    
            else
            {
                return ActionFalseNotData("Get CodeChallenge Fail!");
            }    
        }

        public IHttpActionResult GetZaloCodeVerifier(string subdomain)
        {
            string result = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext(subdomain))
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                HT_CongTy cty = _classHTCT.Gets(null).FirstOrDefault();
                result = cty.ZaloCodeVerifier;
            }
            return ActionTrueNotData(result);
        }

        public IHttpActionResult UpdateAccessTokenAndRefreshTokenZaloApi(string subdomain, string access_token, string refresh_token)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext(subdomain))
                {
                    ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                    HT_CongTy cty = _classHTCT.Gets(null).FirstOrDefault();
                    cty.ZaloAccessToken = access_token;
                    cty.ZaloRefreshToken = refresh_token;
                    _classHTCT.UpdateHTCty(cty);
                }
                return ActionTrueNotData("");
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        public IHttpActionResult GetZaloConnectStatus(string subdomain)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext(subdomain))
                {
                    ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                    HT_CongTy cty = _classHTCT.Gets(null).FirstOrDefault();
                    if(cty.ZaloRefreshToken == null || cty.ZaloRefreshToken == "")
                    {
                        return ActionFalseNotData("");
                    }    
                    else
                    {
                        CZaloApi zaloApi = new CZaloApi();
                        CZaloApi.ZaloOfficialAccountInfoApiResult apiOAResult = zaloApi.GetZaloOfficialAccountInfoApiResult(cty.ZaloAccessToken);
                        if(apiOAResult.error == -216)
                        {
                            CZaloApi.ZaloApiToKen apiToKen = zaloApi.GetAccessTokenFromRefreshToken(cty.ZaloRefreshToken);
                            if (apiToKen.access_token != "")
                            {
                                cty.ZaloAccessToken = apiToKen.access_token;
                                cty.ZaloRefreshToken = apiToKen.refresh_token;
                                _classHTCT.UpdateHTCty(cty);
                                apiOAResult = zaloApi.GetZaloOfficialAccountInfoApiResult(cty.ZaloAccessToken);
                            }
                        }
                        if (apiOAResult.error == 0)
                        {
                            return ActionTrueData(apiOAResult.data);
                        }    
                        
                        else
                        {
                            return ActionFalseNotData("");
                        }
                    }    
                }
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        #endregion
        public List<HT_CongTy> GetHT_CongTy()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                var cty = _classHTCT.Gets(null);
                if (cty != null)
                {
                    var obj = cty.Select(x => new
                    {
                        ID = x.ID,
                        TaiKhoanNH = x.TaiKhoanNganHang,
                        TenCongTy = x.TenCongTy,
                        SoDienThoai = x.SoDienThoai,
                        DiaChiNganHang = x.DiaChiNganHang,
                        DiaChi = x.DiaChi
                    }).AsEnumerable().Select(c => new HT_CongTy
                    {
                        ID = c.ID,
                        TaiKhoanNganHang = c.TaiKhoanNH,
                        TenCongTy = c.TenCongTy,
                        SoDienThoai = c.SoDienThoai,
                        DiaChiNganHang = c.DiaChiNganHang,
                        DiaChi = c.DiaChi
                    }).ToList();
                    return obj;
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<Object> GetHT_CauHinh_TichDiem(Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HT_CauHinh_TichDiem _classHTCHTD = new HT_CauHinh_TichDiem(db);
                var data = _classHTCHTD.GetHT_CauHinh_TichDiem(idDonVi).ToList();
                return data;
            }
        }

        public IHttpActionResult GetHT_CauHinh_TichDiemChiTiet(Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HT_CauHinh_TichDiem _classHTCHTD = new HT_CauHinh_TichDiem(db);
                try
                {
                    var data = _classHTCHTD.SP_GetHT_CauHinh_TichDiem(idDonVi);
                    return Json(new { res = true, data = data });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        public IHttpActionResult GetHT_CauHinh_GioiHanTraHang(Guid idDonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db != null)
            {
                var data = from ct in db.HT_CauHinh_GioiHanTraHang
                           select new
                           {
                               ct.ID,
                               ct.SoNgayGioiHan,
                               ct.ChoPhepTraHang,
                           };

                return Json(new { res = true, data = data });
            }
            else
            {
                return Json(new { res = false, mes = "data null" });
            }
        }

        [HttpPost]
        public IHttpActionResult Post_HT_ThongBao([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                HT_ThongBao objNew = data.ToObject<HT_ThongBao>();

                Guid id = Guid.NewGuid();
                HT_ThongBao objAdd = new HT_ThongBao
                {
                    ID = id,
                    ID_DonVi = objNew.ID_DonVi,
                    LoaiThongBao = objNew.LoaiThongBao,
                    // used to when post thongbao at .js
                    NoiDungThongBao = objNew.NoiDungThongBao.Replace("00000000-0000-0000-0000-000000000000", id.ToString()),
                    NgayTao = DateTime.Now,
                    NguoiDungDaDoc = objNew.NguoiDungDaDoc,
                };

                string strIns = _classHTCT.Insert_HT_ThongBao(objAdd);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = objAdd.ID }, objAdd);
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult Insert_ThongBaoHetTon(libDM_HangHoa.KiemKhoParamSearch param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var idQuyDois = string.Join(",", param.ListIDQuyDoi);
                    var idLoHangs = string.Empty;
                    if (param.ListIDLoHang != null && param.ListIDLoHang.Count > 0)
                    {
                        idLoHangs = string.Join(",", param.ListIDLoHang);
                    }
                    List<SqlParameter> prm = new List<SqlParameter>();
                    prm.Add(new SqlParameter("ID_ChiNhanh", param.ID_ChiNhanh));
                    prm.Add(new SqlParameter("IDDonViQuyDois", idQuyDois));
                    prm.Add(new SqlParameter("IDLoHangs", idLoHangs));
                    var data = db.Database.SqlQuery<TestTonKho>("exec Insert_ThongBaoHetTonKho @ID_ChiNhanh, @IDDonViQuyDois, @IDLoHangs", prm.ToArray()).ToList();
                    return Json(new { res = true , data });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex });
            }
        }

        [HttpDelete, HttpPost]
        public void Delete_HT_ThongBao(string where)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                _classHTCT.Delete_HT_ThongBao(where);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public string GetProcessText([FromBody] JObject objIn)
        {
            string ProgressName = objIn["ProgressName"].ToObject<string>();
            try
            {
                return CookieStore.ReadProgress(ProgressName);
            }
            catch
            {
                return "";
            }
        }

        [AcceptVerbs("GET", "POST")]
        public void ResetProcessText([FromBody] JObject objIn)
        {
            string ProgressName = objIn["ProgressName"].ToObject<string>();
            CookieStore.WriteProgress("0", ProgressName);
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult InitHeaderMenu([FromBody] JObject objIn)
        {
            Guid idnguoidung = Guid.Empty, iddonvi = Guid.Empty;
            if (objIn["IdNguoiDung"] != null && objIn["IdNguoiDung"].ToObject<string>() != "")
                idnguoidung = objIn["IdNguoiDung"].ToObject<Guid>();
            if (objIn["IdDonVi"] != null && objIn["IdDonVi"].ToObject<string>() != "")
                iddonvi = objIn["IdDonVi"].ToObject<Guid>();
            List<HeaderMenu> lst = new List<HeaderMenu>();

            List<string> lstQuyen = new List<string>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<string> lstResult = new List<string>();
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
                        lstResult = htqn.Select(p => p.MaQuyen).ToList();
                    }
                }
                else
                {
                    var htqn = _classHTQuyen.Gets(null);
                    lstResult = htqn.Select(p => p.MaQuyen).ToList();
                }
                lstQuyen = lstResult;
            }
            string NganhNgheKinhDoanh = CookieStore.GetCookieAes("shop").ToUpper();
            var isGara = NganhNgheKinhDoanh == "C16EDDA0-F6D0-43E1-A469-844FAB143014";
            if (isGara == false)
            {
                List<HeaderMenu> lstSubMenuHangHoa = new List<HeaderMenu>();
                bool lstSubMenuHangHoaCheck = false;
                if (lstQuyen.Where(p => p == "HangHoa_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHangHoa.Add(new HeaderMenu(1, "Danh mục", "Danh mục", UrlPage.Danhsachhanghoa, "p/danhsachhanghoa", false, "fa fa-list-alt", new List<HeaderMenu>()));
                    lstSubMenuHangHoaCheck = true;
                }
                if (lstQuyen.Where(p => p == "ThietLapGia_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHangHoa.Add(new HeaderMenu(2, "Bảng giá", "Bảng giá", UrlPage.ThietLapGia, "p/danhsachgia", false, "fa fa-tag", new List<HeaderMenu>()));
                    lstSubMenuHangHoaCheck = true;
                }
                if (lstQuyen.Where(p => p == "PhieuDieuChinh").FirstOrDefault() != null)
                {
                    lstSubMenuHangHoa.Add(new HeaderMenu(3, "Điều chỉnh giá vốn", "Điều chỉnh giá vốn", UrlPage.PhieuDieuChinh, "p/phieudieuchinh", false, "fa fa-search", new List<HeaderMenu>()));
                    lstSubMenuHangHoaCheck = true;
                }
                if (lstQuyen.Where(p => p == "LoHang").FirstOrDefault() != null)
                {
                    lstSubMenuHangHoa.Add(new HeaderMenu(4, "Lô hàng hóa", "Lô hàng hóa", UrlPage.LoHangHoa, "p/LoHangHoa", false, "fa fa-cube", new List<HeaderMenu>()));
                    lstSubMenuHangHoaCheck = true;
                }
                if (lstQuyen.Where(p => p == "KiemKho_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHangHoa.Add(new HeaderMenu(5, "Kiểm kê hàng hóa", "Kiểm kê hàng hóa", UrlPage.KiemKho, "e/kiemkho", false, " fa fa-check-square-o ", new List<HeaderMenu>()));
                    lstSubMenuHangHoaCheck = true;
                }


                List<HeaderMenu> lstSubMenuHoatDong = new List<HeaderMenu>();
                bool lstSubMenuHoatDongCheck = false;
                //if (lstQuyen.Where(p => p == "DanhMucXe_XemDS").FirstOrDefault() != null && NganhNgheKinhDoanh == "C16EDDA0-F6D0-43E1-A469-844FAB143014") //C16EDDA0-F6D0-43E1-A469-844FAB143014 - Gara
                //{
                //    lstSubMenuHoatDong.Add(new HeaderMenu(1, "Danh sách xe", "Danh sách xe", UrlPage.DanhSachXe, "e/DanhSachXe", false, "fa fa-car", new List<HeaderMenu>()));
                //    lstSubMenuHoatDongCheck = true;
                //}
                //if (lstQuyen.Where(p => p == "PhieuTiepNhan_XemDS").FirstOrDefault() != null && NganhNgheKinhDoanh == "C16EDDA0-F6D0-43E1-A469-844FAB143014")
                //{
                //    lstSubMenuHoatDong.Add(new HeaderMenu(2, "Phiếu tiếp nhận", "Phiếu tiếp nhận", UrlPage.DanhSachPhieuTiepNhan, "e/DanhSachPhieuTiepNhan", false, "fa fa-ticket", new List<HeaderMenu>()));
                //    lstSubMenuHoatDongCheck = true;
                //}
                if (lstQuyen.Where(p => p == "DatHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(3, "Đặt hàng", "Đặt hàng", UrlPage.DatHang, "e/dathang", false, "fa fa-cart-plus", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "HoaDon_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(4, "“Đơn sử dụng dịch vụ/sản phẩm”", "“Đơn sử dụng dịch vụ/sản phẩm”", UrlPage.HoaDon, "e/hoadon", false, "fa fa-list", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "GoiDichVu").FirstOrDefault() != null && (NganhNgheKinhDoanh == "AC9DF2ED-FF08-488F-9A64-08433E541020" || NganhNgheKinhDoanh == "83894499-AEFA-4F58-96B4-5EC1A0B16A76"))
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(5, "Đơn hàng", "Đơn hàng", UrlPage.GoiDichVu, "e/GoiDichVu", false, "fa fa-suitcase", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "TheGiaTri_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(6, "Khách hàng đặt cọc", "Khách hàng đặt cọc", UrlPage.NapTienTheGiaTri, "e/NapTienTheGiaTri", false, "fa fa-credit-card", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "TraHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(7, "Đổi trả hàng", "Đổi trả hàng", UrlPage.TraHang, "e/trahang", false, "fa fa-reply", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "NhapHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(8, "Nhập hàng", "Nhập hàng", UrlPage.NhapHang, "e/nhaphang", false, "fa fa-mail-forward", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "TraHangNhap_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(9, "Trả hàng nhập", "Trả hàng nhập", UrlPage.TraHangNhap, "e/trahangnhap", false, "fa fa-reply-all", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "ChuyenHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(10, "Điều chuyển", "Điều chuyển", UrlPage.ChuyenHang, "e/ChuyenHang", false, "fa fa-truck", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }
                if (lstQuyen.Where(p => p == "XuatHuy_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuHoatDong.Add(new HeaderMenu(11, "Xuất kho", "Xuất kho", UrlPage.XuatHuy, "e/XuatHuy", false, "fa fa-recycle", new List<HeaderMenu>()));
                    lstSubMenuHoatDongCheck = true;
                }

                if (lstSubMenuHoatDong.Where(p => p.ID <= 2).ToList().Count > 0)
                {
                    lstSubMenuHoatDong[lstSubMenuHoatDong.Where(p => p.ID <= 2).ToList().Count - 1].EndGroup = true;
                }
                if (lstSubMenuHoatDong.Where(p => p.ID <= 7).ToList().Count > 0)
                {
                    lstSubMenuHoatDong[lstSubMenuHoatDong.Where(p => p.ID <= 7).ToList().Count - 1].EndGroup = true;
                }

                List<HeaderMenu> lstSubMenuDoiTac = new List<HeaderMenu>();
                bool lstSubMenuDoiTacCheck = false;
                if (lstQuyen.Where(p => p == "KhachHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuDoiTac.Add(new HeaderMenu(1, "Khách hàng", "Khách hàng", UrlPage.KhachHang, "c/khachhang", false, "fa fa-users", new List<HeaderMenu>()));
                    lstSubMenuDoiTacCheck = true;
                }
                if (lstQuyen.Where(p => p == "NhaCungCap_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuDoiTac.Add(new HeaderMenu(2, "Nhà cung cấp", "Nhà cung cấp", UrlPage.NhaCungCap, "c/nhacungcap", false, "fa fa-user-plus", new List<HeaderMenu>()));
                    lstSubMenuDoiTacCheck = true;
                }
                if (lstQuyen.Where(p => p == "CongViec_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuDoiTac.Add(new HeaderMenu(3, "Công việc - Lịch hẹn", "Công việc - Lịch hẹn", UrlPage.Congviec, "mark/Congviec", false, "fa fa-briefcase", new List<HeaderMenu>()));
                    lstSubMenuDoiTacCheck = true;
                }
                if (lstQuyen.Where(p => p == "PhanHoi_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuDoiTac.Add(new HeaderMenu(5, "Phản hồi", "Phản hồi", UrlPage.PhanHoi, "s/PhanHoi", false, "fa fa-phone", new List<HeaderMenu>()));
                    lstSubMenuDoiTacCheck = true;
                }
                if (lstQuyen.Where(p => p == "GuiTinNhan_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuDoiTac.Add(new HeaderMenu(6, "Tin nhắn SMS", "Tin nhắn SMS", UrlPage.DichVuSMS, "s/DanhSachSMS", false, "fa fa-comment", new List<HeaderMenu>()));
                    lstSubMenuDoiTacCheck = true;
                }
                if (lstSubMenuDoiTac.Where(p => p.ID <= 2).ToList().Count > 0)
                {
                    lstSubMenuDoiTac[lstSubMenuDoiTac.Where(p => p.ID <= 2).ToList().Count - 1].EndGroup = true;
                }

                List<HeaderMenu> lstSubMenuBaoCao = new List<HeaderMenu>();
                if (lstQuyen.Where(p => p == "BaoCaoBanHang").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(1, "Bán hàng", "Bán hàng", UrlPage.R_BanHang, "x/BanHang", false, "fa fa-bar-chart", new List<HeaderMenu>()));
                }
                //if (lstQuyen.Where(p => p == "BaoCaoSuaChua").FirstOrDefault() != null && NganhNgheKinhDoanh == "C16EDDA0-F6D0-43E1-A469-844FAB143014")
                //{
                //    lstSubMenuBaoCao.Add(new HeaderMenu(2, "Sửa chữa", "Sửa chữa", UrlPage.BaoCaoSuaChua, "g/Gara_DoanhThuCoVan", false, "fa fa-user", new List<HeaderMenu>()));
                //}
                if (lstQuyen.Where(p => p == "BaoCaoDatHang").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(3, "Đặt hàng", "Đặt hàng", UrlPage.R_DatHang, "x/DatHang", false, "fa fa-dot-circle-o", new List<HeaderMenu>()));
                }
                if (lstQuyen.Where(p => p == "BaoCaoNhapHang").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(4, "Nhập hàng", "Nhập hàng", UrlPage.R_NhapHang, "x/nhaphang", false, "fa fa-share", new List<HeaderMenu>()));
                }
                if (lstQuyen.Where(p => p == "BaoCaoKho").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(5, "Kho hàng", "Kho hàng", UrlPage.R_Kho, "x/Kho", false, "fa fa-cubes", new List<HeaderMenu>()));
                }
                if (lstQuyen.Where(p => p == "BaoCaoTaiChinh").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(6, "Tài chính", "Tài chính", UrlPage.R_TaiChinh, "x/taichinh", false, "fa fa-credit-card", new List<HeaderMenu>()));
                }
                if (lstQuyen.Where(p => p == "BaoCaoChietKhau").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(7, "Hoa Hồng", "Hoa Hồng", UrlPage.R_ChietKhau1, "x/HoaHong", false, "fa fa-users", new List<HeaderMenu>()));
                }
                if (lstQuyen.Where(p => p == "BaoCaoTheGiaTri").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(8, "Thẻ giá trị", "Thẻ giá trị", UrlPage.R_TheGiaTri, "x/TheGiaTri", false, "fa fa-usd", new List<HeaderMenu>()));
                }
                if (lstQuyen.Where(p => p == "BaoCaoNhanVien").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(9, "Nhân viên", "Nhân viên", UrlPage.R_NhanVien, "x/NhanVien", false, "fa fa-user", new List<HeaderMenu>()));
                }

                if (lstQuyen.Where(p => p == "BaoCaoGoiDichVu").FirstOrDefault() != null && (NganhNgheKinhDoanh == "AC9DF2ED-FF08-488F-9A64-08433E541020" || NganhNgheKinhDoanh == "83894499-AEFA-4F58-96B4-5EC1A0B16A76"))
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(9, "Gói dịch vụ", "Gói dịch vụ", UrlPage.R_GoiDichVu, "x/GoiDichVu", false, "fa fa-suitcase", new List<HeaderMenu>()));
                }
                List<HeaderMenu> lstSubMenuNhanSu = new List<HeaderMenu>();
                bool lstSubMenuNhanSuCCheck = false;
                if (lstQuyen.Where(p => p == "ChiNhanh_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(1, "Quản lý chi nhánh", "Quản lý chi nhánh", UrlPage.CD_ChiNhanh, "t/QuanLyChiNhanh", false, "fa fa-home", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCCheck = true;
                }
                if (lstQuyen.Where(p => p == "NhanVien_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(2, "Quản lý nhân viên", "Quản lý nhân viên", UrlPage.NhanVien, "t/NhanVien", false, "fa fa-users", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCCheck = true;
                }
                if (lstQuyen.Where(p => p == "NguoiDung_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(3, "Quản lý người dùng", "Quản lý người dùng", UrlPage.CD_NguoiDung, "t/QuanLyNguoiDung", false, "fa fa-user-circle", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCCheck = true;
                }
                if (lstQuyen.Where(p => p == "CaLamViec_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(4, "Danh mục ca làm việc", "Danh mục ca làm việc", UrlPage.CaLamViec, "n/CaLamViec", false, "fa fa-calendar-check-o", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCCheck = true;
                }
                if (lstQuyen.Where(p => p == "PhieuPhanCa_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(5, "Phân công ca làm việc", "Phân công ca làm việc", UrlPage.PhanCongCaLamViec, "n/PhanCongCaLamViec", false, "fa fa-calendar", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCCheck = true;
                }
                if (lstQuyen.Where(p => p == "ChamCong_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(6, "Chấm công", "Chấm công", UrlPage.ChamCong2_1, "n/ChamCong2_1", false, "fa fa-calendar-check-o", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCCheck = true;
                }
                if (lstQuyen.Where(p => p == "BangLuong_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(7, "Bảng lương", "Bảng lương", UrlPage.BangLuong, "n/BangLuong", false, "fa fa-calculator", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCCheck = true;
                }
                if (lstSubMenuNhanSu.Where(p => p.ID <= 3).ToList().Count > 0)
                {
                    lstSubMenuNhanSu[lstSubMenuNhanSu.Where(p => p.ID <= 3).ToList().Count - 1].EndGroup = true;
                }

                //Main menu
                if (lstQuyen.Where(p => p == "TongQuan").FirstOrDefault() != null)
                {
                    lst.Add(new HeaderMenu(1, "Tổng quan", "Tổng quan", UrlPage.TrangChu, "Home/DashBoard", false, "fa fa-laptop", new List<HeaderMenu>()));
                }
                
                if (lstSubMenuHangHoaCheck)
                {
                    lst.Add(new HeaderMenu(2, "Hàng hóa", "Hàng hóa", "", "", false, "fa fa-cubes", lstSubMenuHangHoa));
                }
                if (lstSubMenuHoatDongCheck)
                {
                    lst.Add(new HeaderMenu(3, "Hoạt động", "Hoạt động", "", "", false, "fa fa-shopping-basket ", lstSubMenuHoatDong));
                }
                if (lstSubMenuDoiTacCheck)
                {
                    lst.Add(new HeaderMenu(4, "Đối tác", "Đối tác", "", "", false, "fa fa-users", lstSubMenuDoiTac));
                }
                if (lstQuyen.Where(p => p == "SoQuy_XemDS").FirstOrDefault() != null)
                {
                    lst.Add(new HeaderMenu(5, "Thu chi", "Thu chi", UrlPage.SoQuy2, "f/soquy2", false, "fa fa-usd", new List<HeaderMenu>()));
                }
                lst.Add(new HeaderMenu(6, "Báo cáo", "Báo cáo", "", "", false, "fa fa-newspaper-o", lstSubMenuBaoCao));
                if (lstSubMenuNhanSuCCheck)
                {
                    lst.Add(new HeaderMenu(7, "Nhân sự", "Nhân sự", "", "", false, "fa  fa-user-circle", lstSubMenuNhanSu));
                }
                //if (NganhNgheKinhDoanh == "C16EDDA0-F6D0-43E1-A469-844FAB143014")
                //{
                //    lst.Add(new HeaderMenu(8, "Bàn làm việc", "Bàn làm việc", "BanLamViec", "", false, "bnt-Cashier btn-gara", new List<HeaderMenu>()));
                //}
                if (NganhNgheKinhDoanh == "C1D14B5A-6E81-4893-9F73-E11C63C8E6BC" && lstQuyen.Where(p => p == "NhaBep_TruyCap").FirstOrDefault() != null)
                {
                    lst.Add(new HeaderMenu(9, "Nhà bếp", "Nhà bếp", "/$/kitchen", "", false, " bnt-Cashier btn-kitchent", new List<HeaderMenu>()));
                }
                if ((lstQuyen.Where(p => p == "HoaDon_ThemMoi" || p == "DatHang_ThemMoi" || p == "TraHang_ThemMoi" || p == "GoiDichVu_ThemMoi").FirstOrDefault() != null)
                    && NganhNgheKinhDoanh != "C16EDDA0-F6D0-43E1-A469-844FAB143014")
                {
                    if (NganhNgheKinhDoanh == "C1D14B5A-6E81-4893-9F73-E11C63C8E6BC")
                    {
                        lst.Add(new HeaderMenu(10, "Thu ngân", "Thu ngân", "/$/nhahang", "", false, "bnt-Cashier btn-restaurant", new List<HeaderMenu>()));
                    }
                    else if (NganhNgheKinhDoanh == "AC9DF2ED-FF08-488F-9A64-08433E541020" || NganhNgheKinhDoanh == "83894499-AEFA-4F58-96B4-5EC1A0B16A76")
                    {
                        lst.Add(new HeaderMenu(11, "Thu ngân", "Thu ngân", "/$/banle", "", false, "bnt-Cashier btn-Sale", new List<HeaderMenu>()));
                    }
                    else
                    {
                        lst.Add(new HeaderMenu(12, "Bán hàng", "Bán hàng", "/$/banle", "", false, "bnt-Cashier btn-Sale", new List<HeaderMenu>()));
                    }
                }

            }
            else
            {
                List<HeaderMenu> lstSubMenuSuaChuaXe = new List<HeaderMenu>();
                lstSubMenuSuaChuaXe.Add(new HeaderMenu(1, "Bàn làm việc", "Bàn làm việc", @UrlPage.BanLamViec, "g/BanLamViec", false, "fa fa-car", new List<HeaderMenu>()));
                if (lstQuyen.Where(p => p == "DanhMucXe_XemDS").FirstOrDefault() != null) //C16EDDA0-F6D0-43E1-A469-844FAB143014 - Gara
                {
                    lstSubMenuSuaChuaXe.Add(new HeaderMenu(2, "Danh sách xe", "Danh sách xe", UrlPage.DanhSachXe, "e/DanhSachXe", false, "fa fa-car", new List<HeaderMenu>()));
                }

                if (lstQuyen.Where(p => p == "PhieuTiepNhan_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuSuaChuaXe.Add(new HeaderMenu(3, "Phiếu tiếp nhận", "Phiếu tiếp nhận", UrlPage.DanhSachPhieuTiepNhan, "e/DanhSachPhieuTiepNhan", false, "fa fa-ticket", new List<HeaderMenu>()));
                }

                if (lstQuyen.Where(p => p == "DatHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuSuaChuaXe.Add(new HeaderMenu(4, "Báo giá sửa chữa", "Báo giá sửa chữa", UrlPage.BaoGia, "e/dathang", false, "fa fa-cart-plus", new List<HeaderMenu>()));
                }

                if (lstQuyen.Where(p => p == "HoaDon_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuSuaChuaXe.Add(new HeaderMenu(5, "Hóa đơn sửa chữa", "Hóa đơn sửa chữa", UrlPage.HoaDonSuaChua, "e/hoadon/1", false, "fa fa-list", new List<HeaderMenu>()));
                }
                if (lstQuyen.Where(p => p == "GoiDichVu").FirstOrDefault() != null)
                {
                    lstSubMenuSuaChuaXe.Add(new HeaderMenu(6, "Gói bảo dưỡng", "Gói bảo dưỡng", UrlPage.GoiDichVu, "e/GoiDichVu", false, "fa fa-suitcase", new List<HeaderMenu>()));
                }
                string SubDomain = CookieStore.GetCookieAes("SubDomain").ToUpper();
                List<string> lstSubdomain = new List<string>()
                {
                    "MANDACONS",
                    "0973474985"
                };
                if (lstSubdomain.Contains(SubDomain) )
                {
                    lstSubMenuSuaChuaXe.Add(new HeaderMenu(7, "Phiếu bàn giao xe", "Phiếu bàn giao xe", UrlPage.PhieuBanGiaoXe, "x/PhieuBanGiaoXe", false, "fa fa-book", new List<HeaderMenu>()));
                }

                List<HeaderMenu> lstSubMenuKhachHang = new List<HeaderMenu>();
                bool lstSubMenuKhachHangCheck = false;
                if (lstQuyen.Where(p => p == "KhachHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhachHang.Add(new HeaderMenu(1, "Khách hàng", "Khách hàng", UrlPage.KhachHang, "c/khachhang", false, "fa fa-users", new List<HeaderMenu>()));
                    lstSubMenuKhachHangCheck = true;
                }

                if (lstQuyen.Where(p => p == "NhaCungCap_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhachHang.Add(new HeaderMenu(2, "Nhà cung cấp", "Nhà cung cấp", UrlPage.NhaCungCap, "c/nhacungcap", false, "fa fa-user-plus", new List<HeaderMenu>()));
                    lstSubMenuKhachHangCheck = true;
                }

                if (lstQuyen.Where(p => p == "BaoHiem_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhachHang.Add(new HeaderMenu(3, "Bảo hiểm", "Bảo hiểm", UrlPage.BaoHiem, "c/NhaBaoHiem", false, "fa fa-shield", new List<HeaderMenu>()));
                    lstSubMenuKhachHangCheck = true;
                }

                if (lstQuyen.Where(p => p == "CongViec_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhachHang.Add(new HeaderMenu(4, "Công việc - Lịch hẹn", "Công việc - Lịch hẹn", UrlPage.Congviec, "mark/Congviec", false, "fa fa-briefcase", new List<HeaderMenu>()));
                    lstSubMenuKhachHangCheck = true;
                }

                

                if (lstQuyen.Where(p => p == "LichHen_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhachHang.Add(new HeaderMenu(6, "Lịch nhắc bảo dưỡng", "Lịch nhắc bảo dưỡng", 
                        UrlPage.Gara_LichNhacBaoDuong, "g/Gara_LichNhacBaoDuong", false, "fa fa-calendar", new List<HeaderMenu>()));
                    lstSubMenuKhachHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "PhanHoi_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhachHang.Add(new HeaderMenu(7, "Phản hồi", "Phản hồi", UrlPage.PhanHoi, "s/PhanHoi", false, "fa fa-phone", new List<HeaderMenu>()));
                    lstSubMenuKhachHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "GuiTinNhan_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhachHang.Add(new HeaderMenu(8, "Quản lý SMS", "Quản lý SMS", UrlPage.DichVuSMS, "s/DanhSachSMS", false, "fa fa-comment", new List<HeaderMenu>()));
                    lstSubMenuKhachHangCheck = true;
                }
                if (lstSubMenuKhachHang.Where(p => p.ID <= 3).ToList().Count > 0)
                {
                    lstSubMenuKhachHang[lstSubMenuKhachHang.Where(p => p.ID <= 3).ToList().Count - 1].EndGroup = true;
                }

                List<HeaderMenu> lstSubMenuBanHang = new List<HeaderMenu>();
                bool lstSubMenuBanHangCheck = false;
                if (lstQuyen.Where(p => p == "DatHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuBanHang.Add(new HeaderMenu(1, "Đặt hàng", "Đặt hàng", UrlPage.DatHang, "e/dathang", false, "fa fa-cart-plus", new List<HeaderMenu>()));
                    lstSubMenuBanHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "HoaDon_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuBanHang.Add(new HeaderMenu(2, "Bán hàng", "Bán hàng", UrlPage.HoaDon, "e/hoadon/0", false, "fa fa-list", new List<HeaderMenu>()));
                    lstSubMenuBanHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "TraHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuBanHang.Add(new HeaderMenu(3, "Đổi trả hàng", "Đổi trả hàng", UrlPage.TraHang, "e/trahang", false, "fa fa-reply", new List<HeaderMenu>()));
                    lstSubMenuBanHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "TheGiaTri_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuBanHang.Add(new HeaderMenu(4, "Khách hàng đặt cọc", "Khách hàng đặt cọc", UrlPage.NapTienTheGiaTri, "x/TheGiaTri", false, "fa fa-usd", new List<HeaderMenu>()));
                    lstSubMenuBanHangCheck = true;
                }

                List<HeaderMenu> lstSubMenuMuaHang = new List<HeaderMenu>();
                bool lstSubMenuMuaHangCheck = false;
                if (lstQuyen.Where(p => p == "NhapHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuMuaHang.Add(new HeaderMenu(1, "Nhập hàng", "Nhập hàng", UrlPage.NhapHang, "e/nhaphang", false, "fa fa-mail-forward", new List<HeaderMenu>()));
                    lstSubMenuMuaHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "TraHangNhap_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuMuaHang.Add(new HeaderMenu(2, "Trả hàng nhà cung cấp", "Trả hàng nhà cung cấp", UrlPage.TraHangNhap, "e/trahangnhap", false, "fa fa-reply-all", new List<HeaderMenu>()));
                    lstSubMenuMuaHangCheck = true;
                }

                List<HeaderMenu> lstSubMenuKhoHang = new List<HeaderMenu>();
                bool lstSubMenuKhoHangCheck = false;
                if (lstQuyen.Where(p => p == "HangHoa_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(1, "Hàng hóa", "Hàng hóa", UrlPage.Danhsachhanghoa, "p/danhsachhanghoa", false, "fa fa-list-alt", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "LoHang").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(2, "Lô hàng ", "Lô hàng ", UrlPage.LoHangHoa, "p/LoHangHoa", false, "fa fa-cube", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "PhieuDieuChinh").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(3, "Điều chỉnh giá vốn", "Điều chỉnh giá vốn", UrlPage.PhieuDieuChinh, "p/phieudieuchinh", false, "fa fa-search", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "ChuyenHang_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(4, "Điều chuyển hàng hóa", "Điều chuyển hàng hóa", UrlPage.ChuyenHang, "e/ChuyenHang", false, "fa fa-truck", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "XuatHuy_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(5, "Xuất kho nội bộ", "Xuất kho nội bộ", UrlPage.XuatHuy, "e/XuatHuy", false, "fa fa-recycle", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "KiemKho_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(6, "Kiểm kê hàng hóa", "Kiểm kê hàng hóa", UrlPage.KiemKho, "e/kiemkho", false, " fa fa-check-square-o ", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(7, "Thiết lập khuyến mãi", "Kiểm kê hàng hóa", @UrlPage.CD_KhuyenMai, "t/KhuyenMai", false, " fa fa-check-square-o ", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }
                if (lstQuyen.Where(p => p == "ThietLapGia_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuKhoHang.Add(new HeaderMenu(8, "Thiết lập giá bán", "Thiết lập giá bán", UrlPage.ThietLapGia, "p/danhsachgia", false, "fa fa-tag", new List<HeaderMenu>()));
                    lstSubMenuKhoHangCheck = true;
                }

                List<HeaderMenu> lstSubMenuThuChi = new List<HeaderMenu>();
                bool lstSubMenuThuChiCheck = false;
                if (lstQuyen.Where(p => p == "SoQuy_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuThuChi.Add(new HeaderMenu(1, "Phiếu thu", "Phiếu thu", UrlPage.PhieuThu, "f/soquy2", false, "op-soquy-plus", new List<HeaderMenu>()));
                    lstSubMenuThuChiCheck = true;
                }
                if (lstQuyen.Where(p => p == "SoQuy_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuThuChi.Add(new HeaderMenu(2, "Phiếu chi", "Phiếu chi", UrlPage.PhieuChi, "f/soquy2", false, "op-soquy-minus", new List<HeaderMenu>()));
                    lstSubMenuThuChiCheck = true;
                }
                if (lstQuyen.Where(p => p == "SoQuy_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuThuChi.Add(new HeaderMenu(3, "Sổ quỹ", "Sổ quỹ", UrlPage.SoQuy2, "f/soquy2", false, "fa fa-usd", new List<HeaderMenu>()));
                    lstSubMenuThuChiCheck = true;
                }


                List<HeaderMenu> lstSubMenuNhanSu = new List<HeaderMenu>();
                bool lstSubMenuNhanSuCheck = false;
                if (lstQuyen.Where(p => p == "ChiNhanh_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(1, "Quản lý chi nhánh", "Quản lý chi nhánh", UrlPage.CD_ChiNhanh, "t/QuanLyChiNhanh", false, "fa fa-home", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCheck = true;
                }
                if (lstQuyen.Where(p => p == "NhanVien_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(2, "Quản lý nhân viên", "Quản lý nhân viên", UrlPage.NhanVien, "t/NhanVien", false, "fa fa-users", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCheck = true;
                }
                if (lstQuyen.Where(p => p == "NguoiDung_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(3, "Quản lý người dùng", "Quản lý người dùng", UrlPage.CD_NguoiDung, "t/QuanLyNguoiDung", false, "fa fa-user-circle", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCheck = true;
                }
                if (lstQuyen.Where(p => p == "CaLamViec_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(4, "Danh mục ca làm việc", "Danh mục ca làm việc", UrlPage.CaLamViec, "n/CaLamViec", false, "fa fa-calendar-check-o", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCheck = true;
                }
                if (lstQuyen.Where(p => p == "PhieuPhanCa_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(5, "Phân công ca làm việc", "Phân công ca làm việc", UrlPage.PhanCongCaLamViec, "n/PhanCongCaLamViec", false, "fa fa-calendar", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCheck = true;
                }
                if (lstQuyen.Where(p => p == "ChamCong_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(6, "Chấm công", "Chấm công", UrlPage.ChamCong2_1, "n/ChamCong2_1", false, "fa fa-calendar-check-o", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCheck = true;
                }
                if (lstQuyen.Where(p => p == "BangLuong_XemDS").FirstOrDefault() != null)
                {
                    lstSubMenuNhanSu.Add(new HeaderMenu(7, "Bảng lương", "Bảng lương", UrlPage.BangLuong, "n/BangLuong", false, "fa fa-calculator", new List<HeaderMenu>()));
                    lstSubMenuNhanSuCheck = true;
                }
                if (lstSubMenuNhanSu.Where(p => p.ID <= 3).ToList().Count > 0)
                {
                    lstSubMenuNhanSu[lstSubMenuNhanSu.Where(p => p.ID <= 3).ToList().Count - 1].EndGroup = true;
                }
                List<HeaderMenu> lstSubMenuBaoCao = new List<HeaderMenu>();
                bool lstSubMenuBaoCaoCheck = false;
                if (lstQuyen.Where(p => p == "BaoCaoBanHang").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(1, "Bán hàng", "Bán hàng", UrlPage.R_BanHang, "x/BanHang", false, "fa fa-bar-chart", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoSuaChua").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(2, "Sửa chữa", "Sửa chữa", UrlPage.BaoCaoSuaChua, "g/Gara_DoanhThuCoVan", false, "fa fa-user", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoDatHang").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(3, "Đặt hàng", "Đặt hàng", UrlPage.R_DatHang, "x/DatHang", false, "fa fa-dot-circle-o", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoNhapHang").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(4, "Nhập hàng", "Nhập hàng", UrlPage.R_NhapHang, "x/nhaphang", false, "fa fa-share", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoKho").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(5, "Kho hàng", "Kho hàng", UrlPage.R_Kho, "x/Kho", false, "fa fa-cubes", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoTaiChinh").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(6, "Tài chính", "Tài chính", UrlPage.R_TaiChinh, "x/taichinh", false, "fa fa-credit-card", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoChietKhau").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(7, "Hoa Hồng", "Hoa Hồng", UrlPage.R_ChietKhau1, "x/HoaHong", false, "fa fa-users", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoTheGiaTri").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(8, "Thẻ giá trị", "Thẻ giá trị", UrlPage.R_TheGiaTri, "x/TheGiaTri", false, "fa fa-usd", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoGoiDichVu").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(9, "Gói dịch vụ", "Gói dịch vụ", UrlPage.R_GoiDichVu, "x/GoiDichVu", false, "fa fa-suitcase", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                }
                if (lstQuyen.Where(p => p == "BaoCaoNhanVien").FirstOrDefault() != null)
                {
                    lstSubMenuBaoCao.Add(new HeaderMenu(9, "Nhân viên", "Nhân viên", UrlPage.R_NhanVien, "x/NhanVien", false, "fa fa-user", new List<HeaderMenu>()));
                    lstSubMenuBaoCaoCheck = true;
                } 

                //if (lstQuyen.Where(p => p == "BaoCaoGoiDichVu").FirstOrDefault() != null && (NganhNgheKinhDoanh == "AC9DF2ED-FF08-488F-9A64-08433E541020" || NganhNgheKinhDoanh == "83894499-AEFA-4F58-96B4-5EC1A0B16A76"))
                //{
                //    lstSubMenuBaoCao.Add(new HeaderMenu(9, "Gói dịch vụ", "Gói dịch vụ", UrlPage.R_GoiDichVu, "x/GoiDichVu", false, "fa fa-suitcase", new List<HeaderMenu>()));
                //}
                //Main menu
                if (lstQuyen.Where(p => p == "TongQuan").FirstOrDefault() != null)
                {
                    lst.Add(new HeaderMenu(0, "Tổng quan", "Tổng quan", @UrlPage.TrangChu, "Home/DashBoard", false, "fa fa-laptop", new List<HeaderMenu>()));
                }
                lst.Add(new HeaderMenu(1, "Bàn làm việc", "Bàn làm việc", @UrlPage.BanLamViec, "g/BanLamViec", false, "fa fa-car", new List<HeaderMenu>()));
                lst.Add(new HeaderMenu(2, "Sửa chữa xe", "Sửa chữa xe", "", "", false, "fa fa-wrench", lstSubMenuSuaChuaXe));
                if (lstSubMenuKhachHangCheck)
                {
                    lst.Add(new HeaderMenu(3, "Đối tác", "Đối tác", "", "", false, "fa fa-user", lstSubMenuKhachHang));
                }
                if (lstSubMenuBanHangCheck)
                {
                    lst.Add(new HeaderMenu(4, "Bán hàng", "Bán hàng", "", "", false, "fa fa-cart-plus ", lstSubMenuBanHang));
                }
                if (lstSubMenuMuaHangCheck)
                {
                    lst.Add(new HeaderMenu(5, "Nhập hàng", "Nhập hàng", "", "", false, "fa fa-cart-arrow-down ", lstSubMenuMuaHang));
                }
                if (lstSubMenuKhoHangCheck)
                {
                    lst.Add(new HeaderMenu(6, "Kho hàng", "Kho hàng", "", "", false, "fa fa-cubes ", lstSubMenuKhoHang));
                }
                if (lstSubMenuThuChiCheck)
                {
                    lst.Add(new HeaderMenu(7, "Thu chi", "Thu chi", "", "", false, "fa fa-money ", lstSubMenuThuChi));
                }
                if (lstSubMenuNhanSuCheck)
                {
                    lst.Add(new HeaderMenu(8, "Nhân sự", "Nhân sự", "", "", false, "fa  fa-user-circle-o", lstSubMenuNhanSu));
                }
                if (lstSubMenuBaoCaoCheck)
                {
                    lst.Add(new HeaderMenu(9, "Báo cáo", "Báo cáo", "", "", false, "fa fa-newspaper-o", lstSubMenuBaoCao));
                }

            }
            return ActionTrueData(new
            {
                Menu = lst,
                Quyen = lstQuyen
            });
        }

        //public List<string> GetListQuyenByNguoiDung(Guid idnguoidung, Guid iddonvi)
        //{
        //    using (SsoftvnContext db = SystemDBContext.GetDBContext())
        //    {
        //        List<string> lstResult = new List<string>();
        //        classHT_NhomNguoiDung _classNND = new classHT_NhomNguoiDung(db);
        //        classHT_NguoiDung_Nhom _classNDN = new classHT_NguoiDung_Nhom(db);
        //        classHT_NguoiDung _classND = new classHT_NguoiDung(db);
        //        classHT_Quyen _classHTQuyen = new classHT_Quyen(db);

        //        HT_NguoiDung ngdung = _classND.Get(p => p.ID == idnguoidung);
        //        HT_NhomNguoiDungDTO ct = new HT_NhomNguoiDungDTO();
        //        if (ngdung.LaAdmin == false)
        //        {
        //            HT_NguoiDung_Nhom nguoidungnhom = _classNDN.Gets(p => p.IDNguoiDung == idnguoidung && p.ID_DonVi == iddonvi).FirstOrDefault();
        //            if (nguoidungnhom != null)
        //            {
        //                var htqn = _classND.Select_HT_Quyen_Nhom(nguoidungnhom.IDNhomNguoiDung);
        //                lstResult = htqn.Select(p => p.MaQuyen).ToList();
        //            }
        //        }
        //        else
        //        {
        //            var htqn = _classHTQuyen.Gets(null);
        //            lstResult = htqn.Select(p => p.MaQuyen).ToList();
        //        }
        //        return lstResult;
        //    }
        //}

        #region Email
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GuiGopY([FromBody] JObject objIn)
        {
            try
            {
                string HoVaTen = "";
                string SoDienThoai = "";
                string Subdomain = "";
                string NoiDung = "";
                string Email = "";
                int ModalType = 0;
                if (objIn["HoVaTen"] != null && objIn["HoVaTen"].ToObject<string>() != "")
                    HoVaTen = objIn["HoVaTen"].ToObject<string>();
                if (objIn["SoDienThoai"] != null && objIn["SoDienThoai"].ToObject<string>() != "")
                    SoDienThoai = objIn["SoDienThoai"].ToObject<string>();
                if (objIn["Subdomain"] != null && objIn["Subdomain"].ToObject<string>() != "")
                    Subdomain = objIn["Subdomain"].ToObject<string>();
                if (objIn["NoiDung"] != null && objIn["NoiDung"].ToObject<string>() != "")
                    NoiDung = objIn["NoiDung"].ToObject<string>();
                if (objIn["Email"] != null && objIn["Email"].ToObject<string>() != "")
                    Email = objIn["Email"].ToObject<string>();
                if (objIn["ModalType"] != null && objIn["ModalType"].ToObject<string>() != "")
                    ModalType = objIn["ModalType"].ToObject<int>();
                MailMessage Msg = new MailMessage();
                string subjectF = "";
                if(ModalType == 1)
                {
                    subjectF = "[Góp ý] ";
                }   
                else if(ModalType == 2)
                {
                    subjectF = "[Đặt mua] ";
                }    
                else if(ModalType == 3)
                {
                    subjectF = "[Yêu cầu gia hạn] ";
                }    
                Msg.Subject = "[Góp ý] " + Subdomain;
                Msg.From = new MailAddress("dangky@open24.vn", "Open24.vn");
                Msg.To.Add("dangky@open24.vn");

                Msg.Body = "Họ tên: " + HoVaTen + ".<br />Số điện thoại: " + SoDienThoai +".<br />Email: " + Email + ".<br />Nội dung: " + NoiDung;
                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential("dangky@open24.vn", "Ssoftvn20182018");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(Msg);
                return ActionTrueNotData("");
            }
            catch
            {
                return ActionTrueNotData("");
            }
        }
        #endregion

        #region Mẫu đặt lịch
        public IHttpActionResult GetTemplate(string s) //s - Subdomain
        {
            string htmldata = "";
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Template\\TemplateDatLich\\" + s + "\\index.html";
            if(System.IO.File.Exists(path))
            {
                htmldata = System.IO.File.ReadAllText(path);
                int indexhead = htmldata.IndexOf("<head>");
                if(indexhead >= 0)
                {
                    htmldata = htmldata.Insert(indexhead + 6, "<style>:root {--color-main: #86b7fe;--color-one: #0068ff;"
                        +"--color-two: rgb(13 110 253 / 25 %);--color-three: #ced4da;--color-four: white;}</style>");
                }
                htmldata = htmldata.Replace("datestartvalue", "09:00");
                htmldata = htmldata.Replace("dateendvalue", "20:00");
                htmldata = htmldata.Replace("dateintervalvalue", "60");
                htmldata = htmldata.Replace("datebeforevalue", "60");
                htmldata = htmldata.Replace("subdomainvalue", s);
            }   
            else
            {

            }
            return ActionTrueData(new
            {
                option = new
                {
                    maincolor = "#86b7fe",
                    colorone = "#0068ff",
                    colortwo = "#c2dbfe",
                    colorthree = "#ced4da",
                    colorfour = "#ffffff"
                },
                html = htmldata
            });

        }
        #endregion
    }

    public class PHT_CongTy
    {
        public string TenCongTy { get; set; }
    }
    public class PDM_DonVi
    {
        public string TenDonVi { get; set; }
        public string DienThoaiDonVi { get; set; }
    }
    public class PDM_DoiTuong
    {
        public string TenDoiTuong { get; set; }
        public string DiaChiDoiTuong { get; set; }
        public string DienThoaiDoiTuong { get; set; }
    } 
    public class TestTonKho
    {
        public string MaHangHoa { get; set; }
        public double? TonKho { get; set; }
        public double? TonToiThieu { get; set; }
    }
    public class PBH_HoaDon
    {
        public string MaHoaDon { get; set; }
        public string NgayLapHoaDon { get; set; }
        public string NhanVien { get; set; }
        public double TongTienHang { get; set; }
        public double TongChietKhau { get; set; }
        public double TongThanhToan { get; set; }
    }
    public class PBH_HoaDonChiTiet
    {
        public string TenHangHoa { get; set; }
        public string DonViTinh { get; set; }
        public double GiaBan { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
    }

    public class HeaderMenu
    {
        public HeaderMenu(int iD, string title, string pageTitle, string url, string path, bool endGroup, string classname, List<HeaderMenu> subMenu)
        {
            ID = iD;
            Title = title;
            PageTitle = pageTitle;
            this.url = url;
            this.path = path;
            EndGroup = endGroup;
            SubMenu = subMenu;
            ClassName = classname;
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string PageTitle { get; set; }
        public string url { get; set; }
        public string path { get; set; }
        public bool EndGroup { get; set; }
        public string ClassName { get; set; }
        public List<HeaderMenu> SubMenu { get; set; }
    }
}