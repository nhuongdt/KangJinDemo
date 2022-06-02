using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Model;
using banhang24.Hellper;
using banhang24.Models;
using banhang24.Resources;
using libHT;
using Model_banhang24vn.DAL;
using System.Data.Entity;
using System.Web.Http.Description;
using System.Net.Http;
using System.Net;
using Model_banhang24vn;
using Model.DAL;
using libNS_NhanVien;
using System.Web.Http.Results;
using HT;
using System.Web.Management;
using libDM_DonVi;
using iTextSharp.text.pdf.qrcode;
using System.Text.RegularExpressions;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class ThietLapApiController : BaseApiController
    {
        private SsoftvnContext db;
        public ThietLapApiController()
        {
            db = SystemDBContext.GetDBContext();
        }

        /// <summary>
        /// Thêm mới mẫu in
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult UpdateMauInHoaDon([FromBody] JObject model)
        {
            try
            {
                FIleUpdate obj = model.ToObject<FIleUpdate>();
                var subDomain = CookieStore.GetCookieAes("SubDomain");
                string defaultFolder = HttpContext.Current.Server.MapPath("~/Template/MauIn");
                string folderCus = HttpContext.Current.Server.MapPath("~/Template/MauIn/" + subDomain + "/");

                if (!Directory.Exists(folderCus))
                {
                    Directory.CreateDirectory(folderCus);
                }

                // combine 2 string --> full path
                string targetPath = Path.Combine(folderCus, obj.NameFile);
                if (System.IO.File.Exists(targetPath))
                {
                    System.IO.File.WriteAllText(targetPath, obj.Value);
                }
                else
                {
                    // create file and close after create file
                    File.CreateText(targetPath).Dispose();
                    System.IO.File.WriteAllText(targetPath, obj.Value);
                }
                return Json(new { res = true, mess = "Cập nhật mẫu in thành công." });
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ThietLapAPI_UpdateMauInHoaDon: " + ex.InnerException + ex.Message);
                return Json(new { res = false, mess = ex.InnerException + ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả mẫu in
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public IHttpActionResult LoadListMauIn()
        {
            HT_NguoiDung objUser_Cookies = contant.GetUserCookies();
            var data = new List<MacDinhMauInView>();
            foreach (var o in commonEnum.DanhSachTenMauIn)
            {
                var item = new MacDinhMauInView
                {
                    Key = o.Key,
                    ListSelectMauIn = (from mauin in db.DM_MauIn
                                       join chungtu in db.DM_LoaiChungTu
                                       on mauin.ID_LoaiChungTu equals chungtu.ID
                                       where chungtu.MaLoaiChungTu.Equals(o.Key)
                                       && mauin.ID_DonVi == objUser_Cookies.ID_DonVi
                                       select new ListTypeMauIn
                                       {
                                           Key = mauin.ID,
                                           Value = mauin.TenMauIn,
                                           ChungTuId = chungtu.ID
                                       }).ToList(),
                    selected = (from mauin in db.DM_MauIn
                                join chungtu in db.DM_LoaiChungTu
                                on mauin.ID_LoaiChungTu equals chungtu.ID
                                where chungtu.MaLoaiChungTu.Equals(o.Key)
                                && mauin.ID_DonVi == objUser_Cookies.ID_DonVi
                                && mauin.LaMacDinh
                                select mauin.ID).FirstOrDefault()
                };
                item.ListSelectMauIn.Add(new ListTypeMauIn { Key = new Guid(), Value = "Chọn mẫu in" });
                data.Add(item);
            }
            return Json(data);
        }


        /// <summary>
        /// Update mẫu in
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateDefaultMauIn(List<DefaultMauIn> model)
        {
            try
            {
                HT_NguoiDung objUser_Cookies = contant.GetUserCookies();
                foreach (var item in model)
                {
                    var mauInOld = (from mauin in db.DM_MauIn
                                    join chungtu in db.DM_LoaiChungTu
                                    on mauin.ID_LoaiChungTu equals chungtu.ID
                                    where chungtu.MaLoaiChungTu.Equals(item.MaChungTu)
                                    && mauin.ID_DonVi == objUser_Cookies.ID_DonVi
                                    && mauin.LaMacDinh
                                    && mauin.ID != item.MauInID
                                    select mauin).FirstOrDefault();
                    var mauInNew = db.DM_MauIn.FirstOrDefault(o => !o.LaMacDinh && o.ID == item.MauInID && o.ID_DonVi == objUser_Cookies.ID_DonVi);
                    if (mauInOld != null && mauInNew != null)
                    {
                        mauInOld.LaMacDinh = false;
                    }
                    if (mauInNew != null)
                    {
                        mauInNew.LaMacDinh = true;
                    }
                    else if (mauInOld != null && mauInNew == null)
                    {
                        mauInOld.LaMacDinh = false;
                    }

                }
                db.SaveChanges();
                return Json(new { res = true, mess = NotificationResource.MauInSetupSuccess });
            }
            catch (Exception ex)
            {
                return Json(new { res = false, log = ex.Message, mess = NotificationResource.MauInSetupError });
            }
        }

        /// <summary>
        /// Lấy danh sách mẫu in theo mã chứng từ
        /// </summary>
        /// <param name="typeChungTu"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListMauIn(string typeChungTu, Guid idDonVi)
        {
            var data = from mauin in db.DM_MauIn
                       join ctu in db.DM_LoaiChungTu
                       on mauin.ID_LoaiChungTu equals ctu.ID
                       where ctu.MaLoaiChungTu.Equals(typeChungTu) && mauin.ID_DonVi == idDonVi
                       select new
                       {
                           Key = mauin.ID,
                           Value = mauin.TenMauIn
                       };
            return Json(data);

        }

        /// <summary>
        /// Lấy mẫu in theo id
        /// </summary>
        /// <param name="idMauIn"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetContentFIlePrint(Guid? idMauIn)
        {
            string content1 = "";
            if (idMauIn != null)
            {
                var data = db.DM_MauIn.Where(o => o.ID == idMauIn).FirstOrDefault();
                content1 = data.DuLieuMauIn;
                switch (data.ID_LoaiChungTu)
                {
                    case 9:
                        content1 = ReaplaceMauIn_KiemKe(content1);
                        break;
                    case 18:
                        content1 = ReaplaceMauIn_DieuChinhGV(content1);
                        break;
                    case 22:
                        content1 = ReaplaceMauIn_TheGiaTri(content1);
                        break;
                    case 24:
                        content1 = ReaplaceMauIn_PhieuLuong(content1);
                        break;
                    default:
                        content1 = ReaplaceMauIn(content1);
                        break;
                }
            }
            return Json(content1);
        }

        /// <summary>
        ///  Lấy mẫu in theo id and replace content with {ValueCard}
        /// </summary>
        /// <param name="idMauIn"></param>
        /// <returns></returns>
        public IHttpActionResult GetContentFIlePrint_ValueCard(Guid? idMauIn)
        {
            string content1 = "";
            if (idMauIn != null)
            {
                content1 = db.DM_MauIn.Where(o => o.ID == idMauIn).Select(o => o.DuLieuMauIn).FirstOrDefault();
            }
            return Json(ReaplaceMauIn_TheGiaTri(content1));

        }

        /// <summary>
        /// Lây mẫu in mạc định theo mã chứng từ
        /// </summary>
        /// <param name="maChungTu"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetContentFIlePrintTypeChungTu(string maChungTu, Guid idDonVi, bool printMultiple = false)
        {
            var sReturn = string.Empty;
            var content1 = from mauin in db.DM_MauIn
                           join ctu in db.DM_LoaiChungTu
                           on mauin.ID_LoaiChungTu equals ctu.ID
                           where ctu.MaLoaiChungTu.Equals(maChungTu) && mauin.ID_DonVi == idDonVi
                           select mauin;

            var content2 = content1.Where(x => x.LaMacDinh);

            if (content2 != null && content2.Count() > 0)
            {
                sReturn = content2.FirstOrDefault().DuLieuMauIn;
            }
            else
            {
                if (commonEnum.DanhSachMauInK80.Any(o => o.Key.Equals(maChungTu)))
                {

                    sReturn = GetFileMauIn(commonEnum.DanhSachMauInK80.FirstOrDefault(o => o.Key.Equals(maChungTu)).Value);
                }
                else
                {
                    if (commonEnum.DanhSachMauInA4.Any(o => o.Key.Equals(maChungTu)))
                    {

                        sReturn = GetFileMauIn(commonEnum.DanhSachMauInA4.FirstOrDefault(o => o.Key.Equals(maChungTu)).Value);
                    }
                }
            }

            if (printMultiple)
            {
                return Json(ReaplaceMauIn_Multiple(sReturn, maChungTu));
            }
            else
            {
                switch (maChungTu)
                {
                    case commonEnum.MauInTeamPlates.TheGiaTri:
                        return Json(ReaplaceMauIn_TheGiaTri(sReturn));
                        break;
                    case commonEnum.MauInTeamPlates.PhieuLuong:
                        return Json(ReaplaceMauIn_PhieuLuong(sReturn));
                        break;
                    case commonEnum.MauInTeamPlates.kiemKho:
                        return Json(ReaplaceMauIn_KiemKe(sReturn));
                        break;
                    case commonEnum.MauInTeamPlates.DieuChinh:
                        return Json(ReaplaceMauIn_DieuChinhGV(sReturn));
                        break;
                    default:
                        return Json(ReaplaceMauIn(sReturn));
                }
            }
        }
        private string ReaplaceMauIn_DieuChinhGV(string content1)
        {
            if (content1.IndexOf("{TenHangHoa") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;

                int row1From = temptable.IndexOf("<tr");
                int row1To = temptable.IndexOf("/tr>") - 4;
                string row1Str = temptable.Substring(row1From, row1To);
                string row1 = row1Str;

                int nextRowFrom = row1To;

                row1Str = string.Concat(" <!--ko foreach: $data.CTHoaDonPrint --> ", row1Str);
                if (row1Str.IndexOf("{GiaVon") > -1)
                {
                    row1Str = string.Concat(row1Str, "<!--/ko-->");
                    goto ReplaceDetail;
                }
                else
                {
                    goto CheckRowNext;
                }
            CheckRowNext:
                {
                    int nextRowTo = temptable.IndexOf("<tr", nextRowFrom + 1);
                    if (nextRowTo < 0)
                    {
                        nextRowTo = temptable.LastIndexOf("/tr>") + 5;
                    }
                    string nextRowStr = temptable.Substring(nextRowFrom, nextRowTo - nextRowFrom);
                    string nextRow = nextRowStr;

                    if (nextRowStr.IndexOf("{GiaVon") > -1)
                    {
                        nextRowStr = string.Concat(nextRowStr, "<!--/ko-->");
                        temptable = temptable.Replace(nextRow, nextRowStr);
                        goto ReplaceDetail;
                    }
                    else
                    {
                        nextRowFrom = nextRowTo;
                        goto CheckRowNext;
                    }
                }


            ReplaceDetail:
                {
                    temptable = temptable.Replace(row1, row1Str);

                    temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                    temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                    temptable = temptable.Replace("{TenHangHoa}", "<span data-bind=\"text: TenHangHoa\"></span>");
                    temptable = temptable.Replace("{TenHangHoaThayThe}", "<span data-bind=\"text: TenHangHoaThayThe\"></span>");
                    temptable = temptable.Replace("{GiaVonHienTai}", "<span data-bind=\"text: GiaVonHienTai\"></span>");
                    temptable = temptable.Replace("{GiaVonMoi}", "<span data-bind=\"text: GiaVonMoi\"></span>");
                    temptable = temptable.Replace("{ChenhLech}", "<span data-bind=\"text: ChenhLech\"></span>");
                    temptable = temptable.Replace("{DonViTinh}", "<span data-bind=\"text: TenDonViTinh\"></span>");
                    temptable = temptable.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                    temptable = temptable.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");
                    temptable = temptable.Replace("{TonKho}", "<span data-bind=\"text: TonKho\"></span>");
                    temptable = temptable.Replace("{KThucTe}", "<span data-bind=\"text: KThucTe\"></span>");
                    temptable = temptable.Replace("{SLLech}", "<span data-bind=\"text: SLLech\"></span>");
                    temptable = temptable.Replace("{GiaTriLech}", "<span data-bind=\"text: GiaTriLech\"></span>");
                    temptable = temptable.Replace("{MaLoHang}", "<span data-bind=\"text: MaLoHang\"></span>");
                    temptable = temptable.Replace("{GhiChu}", "<span data-bind=\"text: GhiChu\"></span>");
                    temptable = temptable.Replace("{ThuocTinh_GiaTri}", "<span data-bind=\"text: ThuocTinh_GiaTri\"></span>");
                    temptable = temptable.Replace("{GhiChuHH}", "<span data-bind=\"text: GhiChuHH\"></span>");

                    // xuat huy
                    temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                    temptable = temptable.Replace("{SoLuongHuy}", "<span data-bind=\"text: SoLuongHuy\"></span>");
                    temptable = temptable.Replace("{GiaVon}", "<span data-bind=\"text: GiaVon\"></span>");
                    temptable = temptable.Replace("{GiaTriHuy}", "<span data-bind=\"text: GiaTriHuy\"></span>");

                    content1 = content1.Replace(temptable1, temptable);
                }
            }

            content1 = Replace_ThongTinChung(content1);

            content1 = content1.Replace("{NhanVienBanHang}", "<span data-bind=\"text: InforHDprintf().NhanVienBanHang\"></span>");
            content1 = content1.Replace("{NguoiTao}", "<span data-bind=\"text: $root.InforHDprintf().NguoiTaoHD\"></span>");

            content1 = content1.Replace("{DienGiai}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: $root.InforHDprintf().DienGiai\"></span>");
            content1 = content1.Replace("{TongTienHang}", "<span data-bind=\"text: $root.InforHDprintf().TongTienHang\"></span>");
            content1 = content1.Replace("{TongSoLuongHang}", "<span data-bind=\"text: $root.InforHDprintf().TongSoLuongHang\"></span>");

            // Kiểm kho
            content1 = content1.Replace("{NguoiCanBang}", "<span data-bind=\"text: InforHDprintf().NguoiCanBang\"></span>");
            content1 = content1.Replace("{TrangThaiKK}", "<span data-bind=\"text: InforHDprintf().TrangThaiKK\"></span>");

            content1 = content1.Replace("{NgayCanBang}", "<span data-bind=\"text: InforHDprintf().NgayCanBang\"></span>");
            content1 = content1.Replace("{TongThucTe}", "<span data-bind=\"text: InforHDprintf().TongThucTe\"></span>");
            content1 = content1.Replace("{TongLechTang}", "<span data-bind=\"text: InforHDprintf().TongLechTang\"></span>");
            content1 = content1.Replace("{TongLechGiam}", "<span data-bind=\"text: InforHDprintf().TongLechGiam\"></span>");
            content1 = content1.Replace("{TongChenhLech}", "<span data-bind=\"text: InforHDprintf().TongChenhLech\"></span>");

            return content1;
        }

        private string ReaplaceMauIn_KiemKe(string content1)
        {
            if (content1.IndexOf("{TenHangHoa") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;

                int row1From = temptable.IndexOf("<tr");
                int row1To = temptable.IndexOf("/tr>") - 4;
                string row1Str = temptable.Substring(row1From, row1To);
                string row1 = row1Str;

                int nextRowFrom = row1To;

                row1Str = string.Concat(" <!--ko foreach: $data.CTHoaDonPrint --> ", row1Str);
                if (row1Str.IndexOf("{KThucTe") > -1)
                {
                    row1Str = string.Concat(row1Str, "<!--/ko-->");
                    goto ReplaceDetail;
                }
                else
                {
                    goto CheckRowNext;
                }
            CheckRowNext:
                {
                    int nextRowTo = temptable.IndexOf("<tr", nextRowFrom + 1);
                    if (nextRowTo < 0)
                    {
                        nextRowTo = temptable.LastIndexOf("/tr>") + 5;
                    }
                    string nextRowStr = temptable.Substring(nextRowFrom, nextRowTo - nextRowFrom);
                    string nextRow = nextRowStr;

                    if (nextRowStr.IndexOf("{KThucTe") > -1)
                    {
                        nextRowStr = string.Concat(nextRowStr, "<!--/ko-->");
                        temptable = temptable.Replace(nextRow, nextRowStr);
                        goto ReplaceDetail;
                    }
                    else
                    {
                        nextRowFrom = nextRowTo;
                        goto CheckRowNext;
                    }
                }


            ReplaceDetail:
                {
                    temptable = temptable.Replace(row1, row1Str);

                    temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                    temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                    temptable = temptable.Replace("{TenHangHoa}", "<span data-bind=\"text: TenHangHoa\"></span>");
                    temptable = temptable.Replace("{TenHangHoaThayThe}", "<span data-bind=\"text: TenHangHoaThayThe\"></span>");
                    temptable = temptable.Replace("{GiaVonHienTai}", "<span data-bind=\"text: GiaVonHienTai\"></span>");
                    temptable = temptable.Replace("{GiaVonMoi}", "<span data-bind=\"text: GiaVonMoi\"></span>");
                    temptable = temptable.Replace("{ChenhLech}", "<span data-bind=\"text: ChenhLech\"></span>");
                    temptable = temptable.Replace("{DonViTinh}", "<span data-bind=\"text: TenDonViTinh\"></span>");
                    temptable = temptable.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                    temptable = temptable.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");
                    temptable = temptable.Replace("{TonKho}", "<span data-bind=\"text: TonKho\"></span>");
                    temptable = temptable.Replace("{KThucTe}", "<span data-bind=\"text: KThucTe\"></span>");
                    temptable = temptable.Replace("{SLLech}", "<span data-bind=\"text: SLLech\"></span>");
                    temptable = temptable.Replace("{GiaTriLech}", "<span data-bind=\"text: GiaTriLech\"></span>");
                    temptable = temptable.Replace("{MaLoHang}", "<span data-bind=\"text: MaLoHang\"></span>");
                    temptable = temptable.Replace("{GhiChu}", "<span data-bind=\"text: GhiChu\"></span>");
                    temptable = temptable.Replace("{ThuocTinh_GiaTri}", "<span data-bind=\"text: ThuocTinh_GiaTri\"></span>");
                    temptable = temptable.Replace("{GhiChuHH}", "<span data-bind=\"text: GhiChuHH\"></span>");

                    // xuat huy
                    temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                    temptable = temptable.Replace("{SoLuongHuy}", "<span data-bind=\"text: SoLuongHuy\"></span>");
                    temptable = temptable.Replace("{GiaVon}", "<span data-bind=\"text: GiaVon\"></span>");
                    temptable = temptable.Replace("{GiaTriHuy}", "<span data-bind=\"text: GiaTriHuy\"></span>");

                    content1 = content1.Replace(temptable1, temptable);
                }
            }

            content1 = Replace_ThongTinChung(content1);

            content1 = content1.Replace("{NhanVienBanHang}", "<span data-bind=\"text: InforHDprintf().NhanVienBanHang\"></span>");
            content1 = content1.Replace("{NguoiTao}", "<span data-bind=\"text: $root.InforHDprintf().NguoiTaoHD\"></span>");
            content1 = content1.Replace("{DienGiai}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: $root.InforHDprintf().DienGiai\"></span>");
            content1 = content1.Replace("{TongTienHang}", "<span data-bind=\"text: $root.InforHDprintf().TongTienHang\"></span>");
            content1 = content1.Replace("{TongSoLuongHang}", "<span data-bind=\"text: $root.InforHDprintf().TongSoLuongHang\"></span>");

            // Kiểm kho
            content1 = content1.Replace("{NguoiCanBang}", "<span data-bind=\"text: InforHDprintf().NguoiCanBang\"></span>");
            content1 = content1.Replace("{TrangThaiKK}", "<span data-bind=\"text: InforHDprintf().TrangThaiKK\"></span>");

            content1 = content1.Replace("{NgayCanBang}", "<span data-bind=\"text: InforHDprintf().NgayCanBang\"></span>");
            content1 = content1.Replace("{TongThucTe}", "<span data-bind=\"text: InforHDprintf().TongThucTe\"></span>");
            content1 = content1.Replace("{TongLechTang}", "<span data-bind=\"text: InforHDprintf().TongLechTang\"></span>");
            content1 = content1.Replace("{TongLechGiam}", "<span data-bind=\"text: InforHDprintf().TongLechGiam\"></span>");
            content1 = content1.Replace("{TongChenhLech}", "<span data-bind=\"text: InforHDprintf().TongChenhLech\"></span>");

            return content1;
        }

        public string Replace_ThongTinChung(string content)
        {
            content = content.Replace("{TenCuaHang}", "<span data-bind=\"text: InforHDprintf().TenCuaHang\"></span>");
            content = content.Replace("{DiaChiCuaHang}", "<span data-bind=\"text: InforHDprintf().DiaChiCuaHang\"></span>");
            content = content.Replace("{DienThoaiCuaHang}", "<span data-bind=\"text: InforHDprintf().DienThoaiCuaHang\"></span>");
            content = content.Replace("{TenChiNhanh}", "<span data-bind=\"text: InforHDprintf().TenChiNhanh\"></span>");
            content = content.Replace("{DienThoaiChiNhanh}", "<span data-bind=\"text: InforHDprintf().DienThoaiChiNhanh\"></span>");
            content = content.Replace("{DiaChiChiNhanh}", "<span data-bind=\"text: InforHDprintf().DiaChiChiNhanh\"></span>");
            content = content.Replace("{Logo}", "<img data-bind=\"attr: {src: InforHDprintf().LogoCuaHang}\" style=\"width:100%\" />");

            content = content.Replace("{NgayTao}", "<span data-bind=\"text: InforHDprintf().NgayTao\"></span>");
            content = content.Replace("{NgayBan}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
            content = content.Replace("{NgayLapHoaDon}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
            content = content.Replace("{NgayApDungGoiDV}", "<span data-bind=\"text: InforHDprintf().NgayApDungGoiDV\"></span>");
            content = content.Replace("{HanSuDungGoiDV}", "<span data-bind=\"text: InforHDprintf().HanSuDungGoiDV\"></span>");
            content = content.Replace("{MaHoaDon}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            return content;
        }
        public string Replace_TheoNhom(string content)
        {
            content = content.Replace("{TenNhomHangHoa}",
                                " <span data-bind=\"text: TenNhomHangHoa\"> </span>");
            content = content.Replace("{SoThuTuNhom}",
                " <span data-bind=\"text: SoThuTuNhom\"> </span>");
            content = content.Replace("{SoThuTuNhom_LaMa}",
             " <span data-bind=\"text: SoThuTuNhom_LaMa\"> </span>");

            content = content.Replace("{TongTienTheoNhom}",
                "<span data-bind=\"text: formatNumber(TongTienTheoNhom)\" > </span>");
            content = content.Replace("{TongTienTheoNhom_TruocVAT}",
            "<span data-bind=\"text: formatNumber(TongTienTheoNhom_TruocVAT)\" > </span>");
            content = content.Replace("{TongTienTheoNhom_TruocCK}",
            "<span data-bind=\"text: formatNumber(TongTienTheoNhom_TruocCK)\" > </span>");

            content = content.Replace("{TongSLTheoNhom}",
               "<span data-bind=\"text: formatNumber(TongSLTheoNhom)\" > </span>");
            content = content.Replace("{TongThueTheoNhom}",
            "<span data-bind=\"text: formatNumber(TongThueTheoNhom)\" > </span>");
            content = content.Replace("{TongCKTheoNhom}",
            "<span data-bind=\"text: formatNumber(TongCKTheoNhom)\" > </span>");
            content = content.Replace("{TheoNhomHang}", "");
            return content;
        }
        public string Replace_HangMucSC(string content)
        {
            if (content.IndexOf("{TinhTrang}") > -1 || content.IndexOf("{TenHangMuc}") > -1 || content.IndexOf("{PhuongAnSuaChua}") > -1)
            {
                int hm_from = -1, hm_to = -1;
                if (content.IndexOf("{TenHangMuc}") > -1)
                {
                    hm_from = content.LastIndexOf("tbody", content.IndexOf("{TenHangMuc}")) - 1;
                    hm_to = content.IndexOf("tbody", content.IndexOf("{TenHangMuc}")) + 6;
                }
                else
                {
                    if (content.IndexOf("{TinhTrang}") > -1)
                    {
                        hm_from = content.LastIndexOf("tbody", content.IndexOf("{TinhTrang}")) - 1;
                        hm_to = content.IndexOf("tbody", content.IndexOf("{TinhTrang}")) + 6;
                    }
                }
                if (hm_from != -1 && hm_to != -1)
                {
                    var hm_tbl = content.Substring(hm_from, hm_to - hm_from);
                    var hm_tblGoc = hm_tbl;
                    hm_tbl = hm_tbl.Replace("tbody", "tbody data-bind=\"foreach: HangMucSuaChua\"");
                    hm_tbl = hm_tbl.Replace("{STT}", "<span data-bind=\"text: STT\"></span>");
                    hm_tbl = hm_tbl.Replace("{TenHangMuc}", "<span data-bind=\"text: TenHangMuc\"></span>");
                    hm_tbl = hm_tbl.Replace("{TinhTrang}", "<span data-bind=\"text: TinhTrang\"></span>");
                    hm_tbl = hm_tbl.Replace("{PhuongAnSuaChua}", "<span data-bind=\"text: PhuongAnSuaChua\"></span>");
                    content = content.Replace(hm_tblGoc, hm_tbl);
                }
            }
            return content;
        }
        public string Replace_VatDungKemTheo(string content)
        {
            if (content.IndexOf("{TieuDe}") != -1)
            {
                var vd_from = content.LastIndexOf("tbody", content.IndexOf("{TieuDe}")) - 1;
                var vd_to = content.IndexOf("tbody", vd_from + 5) + 6;
                var vd_tbl = content.Substring(vd_from, vd_to - vd_from);
                var vd_tblGoc = vd_tbl;
                vd_tbl = vd_tbl.Replace("tbody", "tbody data-bind=\"foreach: VatDungKemTheo\"");
                vd_tbl = vd_tbl.Replace("{STT}", "<span data-bind=\"text: STT\"></span>");
                vd_tbl = vd_tbl.Replace("{TieuDe}", "<span data-bind=\"text: TieuDe\"></span>");
                vd_tbl = vd_tbl.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                content = content.Replace(vd_tblGoc, vd_tbl);
            }
            return content;
        }
        public string Replace_ThongTinHangHoa(string content)
        {
            content = content.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
            content = content.Replace("{TenHangHoa}", "<span data-bind=\"text: TenHangHoa\"></span>");
            content = content.Replace("{TenHangHoaThayThe}", "<span data-bind=\"text: TenHangHoaThayThe\"></span>");
            content = content.Replace("{DonGiaBaoHiem}", "<span data-bind=\"text: formatNumber(DonGiaBaoHiem)\"></span>");
            content = content.Replace("{DonViTinh}", "<span data-bind=\"text: TenDonViTinh\"></span>");
            content = content.Replace("{DonGia}", "<span data-bind=\"text: formatNumber(DonGia)\"></span>");
            content = content.Replace("{GiaBan}", "<span data-bind=\"text: formatNumber(GiaBan)\"></span>");
            content = content.Replace("{GiamGia}", "<span data-bind=\"text: formatNumber(TienChietKhau)\"></span>");
            content = content.Replace("{SoLuong}", "<span data-bind=\"text: formatNumber(SoLuong)\"></span>");
            content = content.Replace("{ThanhTien}", "<span data-bind=\"text: formatNumber(ThanhTien)\"></span>");
            content = content.Replace("{ThanhTienTruocCK}", "<span data-bind=\"text: formatNumber(ThanhTienTruocCK)\"></span>");
            content = content.Replace("{TienThue}", "<span data-bind=\"text: formatNumber(TienThue)\"></span>");
            content = content.Replace("{HH_ThueTong}", "<span data-bind=\"text: formatNumber(HH_ThueTong)\"></span>");
            content = content.Replace("{ThanhToan}", "<span data-bind=\"text: formatNumber(ThanhToan)\"></span>");
            content = content.Replace("{MaLoHang}", "<span data-bind=\"text: MaLoHang\"></span>");
            content = content.Replace("{GhiChu}", "<span data-bind=\"text: GhiChu\"></span>");
            content = content.Replace("{ThuocTinh_GiaTri}", "<span data-bind=\"text: ThuocTinh_GiaTri\"></span>");
            content = content.Replace("{PTChietKhauHH}", "<span data-bind=\"text: formatNumber(PTChietKhau)\"></span>");
            content = content.Replace("{PTThue}", "<span data-bind=\"text: formatNumber(PTThue)\"></span>");
            content = content.Replace("{GhiChuHH}", "<span data-bind=\"text: GhiChuHH\"></span>");
            content = content.Replace("{BH_ThanhTien}", "<span data-bind=\"text: formatNumber(BH_ThanhTien)\"></span>");
            content = content.Replace("{PTChiPhi}", "<span data-bind=\"text: formatNumber(PTChiPhi)\"></span>");
            content = content.Replace("{TienChiPhi}", "<span data-bind=\"text: formatNumber(TienChiPhi)\"></span>");
            content = content.Replace("{TongChietKhau}", "<span data-bind=\"text: formatNumber(TongChietKhau)\"></span>");

            // sudung dv
            content = content.Replace("{SLDVDaSuDung}", "<span data-bind=\"text: formatNumber(SoLuongDVDaSuDung)\"></span>");
            content = content.Replace("{SLDVConLai}", "<span data-bind=\"text: formatNumber(SoLuongDVConLai)\"></span>");
            content = content.Replace("{SoLuongMua}", "<span data-bind=\"text: formatNumber(SoLuongMua)\"></span>");
            content = content.Replace("{SoPhutThucHien}", "<span data-bind=\"text: DichVuTheoGio==1? ConvertMinutes_ToHourMinutes(ThoiGianThucHien): ThoiGianThucHien\"></span>");
            content = content.Replace("{ThoiGianBatDau}", "<span data-bind=\"text: TimeStart\"></span>");
            content = content.Replace("{QuaThoiGian}", "<span data-bind=\"text: QuaThoiGian\"></span>");
            content = content.Replace("{TenViTri}", "<span data-bind=\"text: TenViTri\"></span>");

            // chi tiet chuyen hang
            content = content.Replace("{SoLuongChuyen}", "<span data-bind=\"text: formatNumber(SoLuongChuyen)\"></span>");
            content = content.Replace("{SoLuongNhan}", "<span data-bind=\"text: formatNumber(SoLuongNhan)\"></span>");
            content = content.Replace("{GiaChuyen}", "<span data-bind=\"text: formatNumber(GiaChuyen)\"></span>");

            // xuat huy
            content = content.Replace("{SoLuongHuy}", "<span data-bind=\"text: formatNumber(SoLuongHuy)\"></span>");
            content = content.Replace("{GiaVon}", "<span data-bind=\"text: formatNumber(GiaVon)\"></span>");
            content = content.Replace("{GiaTriHuy}", "<span data-bind=\"text: formatNumber(GiaTriHuy)\"></span>");

            // spa
            content = content.Replace("{GhiChu_NVThucHien}", "<span data-bind=\"text: GhiChu_NVThucHienPrint\"></span>");
            content = content.Replace("{GhiChu_NVTuVan}", "<span data-bind=\"text: GhiChu_NVTuVanPrint\"></span>");
            content = content.Replace("{NVThucHienDV_CoCK}", "<span data-bind=\"text: NVThucHienDV_CoCK\"></span>");
            content = content.Replace("{NVTuVanDV_CoCK}", "<span data-bind=\"text: NVTuVanDV_CoCK\"></span>");
            return content;
        }

        /// <summary>
        /// Hàm replace Các tham số để bind dữ liệu khi in
        /// </summary>
        /// <param name="content1"></param>
        /// <returns></returns>
        private string ReaplaceMauIn(string content1)
        {
            if (content1.IndexOf("{TenHangHoaMoi}") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa}")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa}")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;

                temptable = temptable.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrint\"");
                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable = Replace_ThongTinHangHoa(temptable);
                content1 = content1.Replace(temptable1, temptable);

                var openTbl2 = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) - 1;
                var closeTbl2 = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) + 6;
                var temptable2 = content1.Substring(openTbl2, closeTbl2 - openTbl2);
                var temptableMH = temptable2;

                temptable2 = temptable2.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrintMH\"");
                temptable2 = temptable2.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable2 = temptable2.Replace("{TenHangHoaMoi}", "<span data-bind=\"text: TenHangHoa\"></span>");
                temptable2 = Replace_ThongTinHangHoa(temptable2);
                content1 = content1.Replace(temptableMH, temptable2);

                content1 = content1.Replace("{TongTienTraHang}", "<span data-bind=\"text: InforHDprintf().TongTienTraHang\"></span>");
                content1 = content1.Replace("{TongTienHoaDonMua}", "<span data-bind=\"text: InforHDprintf().TongTienHoaDonMua\"></span>");
                content1 = content1.Replace("{TienTraKhach}", "<span data-bind=\"text: formatNumber(InforHDprintf().PhaiTraKhach)\"></span>");
            }
            else
            if (content1.IndexOf("{Nhom_HangHoaDV}") == -1 && (content1.IndexOf("{TheoHangHoa}") > -1 || content1.IndexOf("{TheoDichVu}") > -1))
            {
                var openTbl = content1.LastIndexOf("<tbody", content1.IndexOf("{TheoHangHoa}")) + 7;
                var closeTbl = content1.IndexOf("tbody>", content1.IndexOf("{TheoDichVu}")) + 6;
                string temptable = content1.Substring(openTbl, closeTbl - openTbl);
                string temptable1 = temptable;

                var strDV = "<!-- ko foreach:  $root.CTHoaDonPrint().filter(x=> x.LaHangHoa === false) -->";
                var strHH = "<!-- ko foreach:  $root.CTHoaDonPrint().filter(x=> x.LaHangHoa) -->";

                var indexHH = temptable1.IndexOf("TheoHangHoa");
                var indexDV = temptable1.IndexOf("TheoDichVu");

                int row1From = temptable.IndexOf("<tr");
                int row1To = temptable.IndexOf("/tr>");
                string row1Str = temptable.Substring(row1From, row1To);
                string row1 = row1Str;
                row1Str = row1Str.Replace("<tr", " <tr data-bind=\"visible: $index()===0\"");

                if (indexHH < indexDV)
                {
                    row1Str = string.Concat(strHH, row1Str);
                }
                else
                {
                    row1Str = string.Concat(strDV, row1Str);
                }

                int row2From = temptable.IndexOf("<tr", temptable.IndexOf("<tr") + 1);
                int row2To = temptable.IndexOf("<tr", row2From + 1);
                string row2Str = temptable.Substring(row2From, row2To - row2From);
                string row2 = row2Str;
                row2Str = string.Concat(row2Str, "<!--/ko--> ");

                // use other
                if (temptable.IndexOf("{TongTienPhuTung") == -1)
                {
                    // find tr3
                    int row3From = row2To;
                    int row3To = temptable.IndexOf("<tr", row3From + 1);
                    string row3Str = temptable.Substring(row3From, row3To - row3From);
                    string row3 = row3Str;
                    row3Str = row3Str.Replace("<tr",
                        " <tr data-bind=\"visible: $index()===0\"");

                    if (indexHH < indexDV)
                    {
                        row3Str = string.Concat(strDV, row3Str);
                    }
                    else
                    {
                        row3Str = string.Concat(strHH, row3Str);
                    }

                    int row4From = row3To;
                    int row4To = temptable.IndexOf("<tr", row4From + 1);
                    if (row4To == -1)
                    {
                        row4To = temptable.LastIndexOf("tr>") + 3;
                    }
                    string row4Str = temptable.Substring(row4From, row4To - row4From);
                    string row4 = row4Str;
                    row4Str = string.Concat(row4Str, "<!--/ko--> ");

                    temptable = temptable.Replace(row1, row1Str);
                    temptable = temptable.Replace(row2, row2Str);
                    temptable = temptable.Replace(row3, row3Str);
                    if (row2.Replace("\n", "") != row4.Replace("\n", ""))
                    {
                        temptable = temptable.Replace(row4, row4Str);
                    }
                }
                else
                {
                    // row3: tongtienphutung
                    int row3From = row2To;
                    int row3To = temptable.IndexOf("<tr", row3From + 1);
                    string row3Str = temptable.Substring(row3From, row3To - row3From);

                    int row4From = row3To;
                    int row4To = temptable.IndexOf("<tr", row4From + 1);
                    string row4Str = temptable.Substring(row4From, row4To - row4From);
                    string row4 = row4Str;
                    row4Str = row4Str.Replace("<tr", " <tr data-bind=\"visible: $index()===0\"");

                    int row5From = row4To;
                    int row5To = temptable.IndexOf("<tr", row5From + 1);
                    string row5Str = temptable.Substring(row5From, row5To - row5From);
                    string row5 = row5Str;
                    row5Str = string.Concat(row5Str, "<!--/ko--> ");

                    if (indexHH < indexDV)
                    {
                        row4Str = string.Concat(strDV, row4Str);
                    }
                    else
                    {
                        row4Str = string.Concat(strHH, row4Str);
                    }
                    temptable = temptable.Replace(row1, row1Str);
                    temptable = temptable.Replace(row2, row2Str);
                    temptable = temptable.Replace(row4, row4Str);
                    // neu style hanghoa = dichvu: chi can replace 1 lan o row2
                    if (row2.Replace("\n", "") != row5.Replace("\n", ""))
                    {
                        temptable = temptable.Replace(row5, row5Str);
                    }
                }

                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: $index() + 1\"></span>");
                temptable = Replace_ThongTinHangHoa(temptable);

                temptable = temptable.Replace("{TheoDichVu}", "");
                temptable = temptable.Replace("{TheoHangHoa}", "");
                content1 = content1.Replace(temptable1, temptable);
            }
            else
            {
                if (content1.IndexOf("{TheoNhomHang}") > -1)
                {
                    int open = content1.LastIndexOf("tbody", content1.IndexOf("{TenNhomHangHoa}")) - 1;
                    int close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa")) + 6;
                    string temptable = content1.Substring(open, close - open);
                    string temptable1 = temptable;

                    int row1From = temptable.IndexOf("<tr");
                    int row1To = temptable.IndexOf("/tr>") - 3;
                    string row1Str = temptable.Substring(row1From, row1To);
                    string row1 = row1Str;
                    row1 = string.Concat(" <!--ko foreach: CTHoaDonPrint_TheoNhom --> ", row1);

                    int row2From = temptable.IndexOf("<tr", temptable.IndexOf("<tr") + 1);
                    int row2To = temptable.IndexOf("<tr", row2From + 1);
                    string row2Str = string.Empty;
                    string row2 = string.Empty;

                    if (row2To < 0)
                    {
                        goto CheckRowNext;
                    }

                    row2Str = temptable.Substring(row2From, row2To - row2From);
                    row2 = row2Str;

                    int row3From = row2To;
                    int row3To = temptable.IndexOf("<tr", row3From + 1);
                    if (row3To < 0)
                    {
                        row2From = row3From;
                        goto CheckRowNext;
                    }
                    else
                    {
                        row2 = string.Concat(" <!--ko foreach: $data.HangHoas --> ", row2, " <!--/ko-->");

                        string row3Str = temptable.Substring(row3From, row3To - row3From);
                        string row3 = row3Str;
                        if (temptable.IndexOf("{TongTienTheoNhom") > -1)
                        {
                            row3 = string.Concat(row3, " <!--/ko --> ");
                        }
                        else
                        {
                            row3 = string.Concat(row3Str, " <!--/ko --> ");
                        }
                        temptable = temptable.Replace(row3Str, row3);
                        temptable = temptable.Replace(row2Str, row2);
                        goto ReplaceDetail;
                    }

                CheckRowNext:
                    {
                        var lastRow = temptable.LastIndexOf("/tr>") + 5;
                        string lastStr = temptable.Substring(row2From, lastRow - row2From);
                        string last = lastStr;

                        if (lastStr.IndexOf("{TongTienTheoNhom") > -1)
                        {
                            row2 = string.Concat(" <!--ko foreach: $data.HangHoas --> ", row2, " <!--/ko-->");
                            temptable = temptable.Replace(row2Str, row2);
                            last = string.Concat(last, " <!--/ko --> ");
                        }
                        else
                        {
                            row1 = string.Concat(row1, " <!--ko foreach: $data.HangHoas --> ");
                            // khong co cot tongtien theonhom
                            last = string.Concat(last, " <!--/ko-->", " <!--/ko-->");
                        }
                        temptable = temptable.Replace(lastStr, last);
                    }

                ReplaceDetail:

                    temptable = temptable.Replace(row1Str, row1);

                    temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                    temptable = Replace_ThongTinHangHoa(temptable);
                    content1 = content1.Replace(temptable1, temptable);

                    content1 = content1.Replace("{TenNhomHangHoa}",
                        " <span data-bind=\"text: TenNhomHangHoa\"> </span>");
                    content1 = content1.Replace("{SoThuTuNhom}",
                        " <span data-bind=\"text: SoThuTuNhom\"> </span>");
                    content1 = content1.Replace("{SoThuTuNhom_LaMa}",
                     " <span data-bind=\"text: SoThuTuNhom_LaMa\"> </span>");

                    content1 = content1.Replace("{TongTienTheoNhom}",
                        "<span data-bind=\"text: formatNumber(TongTienTheoNhom)\" > </span>");
                    content1 = content1.Replace("{TongTienTheoNhom_TruocVAT}",
                    "<span data-bind=\"text: formatNumber(TongTienTheoNhom_TruocVAT)\" > </span>");
                    content1 = content1.Replace("{TongTienTheoNhom_TruocCK}",
                    "<span data-bind=\"text: formatNumber(TongTienTheoNhom_TruocCK)\" > </span>");

                    content1 = content1.Replace("{TongSLTheoNhom}",
                       "<span data-bind=\"text: formatNumber(TongSLTheoNhom)\" > </span>");
                    content1 = content1.Replace("{TongThueTheoNhom}",
                    "<span data-bind=\"text: formatNumber(TongThueTheoNhom)\" > </span>");
                    content1 = content1.Replace("{TongCKTheoNhom}",
                    "<span data-bind=\"text: formatNumber(TongCKTheoNhom)\" > </span>");
                    content1 = content1.Replace("{TheoNhomHang}", "");
                }
                else
                {
                    if (content1.IndexOf("{Combo}") > -1)
                    {
                        int open = content1.LastIndexOf("tbody", content1.IndexOf("{Combo}")) - 1;
                        int close = content1.IndexOf("tbody", content1.IndexOf("{Combo}")) + 6;
                        string temptable = content1.Substring(open, close - open);
                        string temptable1 = temptable;

                        int row1From = temptable.IndexOf("<tr");
                        int row1To = temptable.IndexOf("/tr>") - 3;
                        string row1Str = temptable.Substring(row1From, row1To);

                        int row2From = temptable.IndexOf("<tr", row1From + 1);
                        int row2To = temptable.IndexOf("<tr", row2From + 1);
                        string row2Str = temptable.Substring(row2From, row2To - row2From);
                        string row2 = string.Concat(" <!--ko foreach: CTHoaDonPrint --> ", row2Str);

                        int nextRowFrom = row2To;
                        int nextRowTo;
                        string nextRowStr;

                    CheckRow3:
                        {
                            nextRowTo = temptable.IndexOf("<tr", nextRowFrom + 1);
                            nextRowStr = temptable.Substring(nextRowFrom, nextRowTo - nextRowFrom);

                            if (nextRowTo > 0)
                            {
                                goto CheckRowTP;
                            }
                            else
                            {
                                goto ReplaceDetail;
                            }
                        }

                    CheckRowTP:
                        {
                            if (nextRowStr.IndexOf("{ThanhPhan}") > -1)
                            {
                                int row4From = nextRowTo;
                                int row4To = temptable.IndexOf("<tr", row4From + 1);
                                if (row4To < 0)
                                {
                                    row4To = temptable.LastIndexOf("/tr>") + 5;
                                }
                                else
                                {
                                    var rNextTo = temptable.IndexOf("<tr", row4To + 1);
                                    if (rNextTo < 0)
                                    {
                                        rNextTo = temptable.LastIndexOf("/tr>") + 5;
                                    }
                                    var strTemp = temptable.Substring(row4From, rNextTo - row4From);
                                    if (strTemp.IndexOf("{GhiChu_NVThucHien}") > -1 || strTemp.IndexOf("{GhiChu_NVTuVan}") > -1)
                                    {
                                        row4To = rNextTo;
                                    }
                                }

                                string row4Str = temptable.Substring(row4From, row4To - row4From);
                                string row4 = string.Concat(" <!--ko foreach: $data.ThanhPhanComBo --> ", row4Str,
                                    "<!--/ko--> <!--/ko-->");

                                temptable = temptable.Replace(row4Str, row4);
                                temptable = temptable.Replace(nextRowStr, ""); // delete row contains {ThanhPhan}
                                goto ReplaceDetail;
                            }
                            else
                            {
                                nextRowFrom = nextRowTo;
                                goto CheckRow3;
                            }
                        }

                    ReplaceDetail:
                        {
                            // delete row1
                            temptable = temptable.Replace(row1Str, "");
                            temptable = temptable.Replace(row2Str, row2);

                            temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                            temptable = Replace_ThongTinHangHoa(temptable);
                            content1 = content1.Replace(temptable1, temptable);
                        }
                    }
                    else
                    {
                        if (content1.IndexOf("{TheoHangHoa_Nhom}") > -1 || content1.IndexOf("{TheoDichVu_Nhom}") > -1)
                        {
                            #region tblHangHoa
                            var startHH = content1.IndexOf("{TheoHangHoa_Nhom}");
                            var opentblHH = content1.IndexOf("tbody", startHH) - 1;
                            var closeblHH = content1.IndexOf("tbody", opentblHH + 6);
                            var sTblHH = content1.Substring(opentblHH, closeblHH - opentblHH + 6);
                            var sTblHH_goc = sTblHH;

                            var hh_headerFrom = content1.IndexOf("thead", startHH) - 1;
                            var hh_headerTo = content1.IndexOf("thead", hh_headerFrom + 5);
                            var hh_sHeader = content1.Substring(hh_headerFrom, hh_headerTo - hh_headerFrom + 6);
                            var hh_sHeaderGoc = hh_sHeader;

                            int hh_row1From = sTblHH.IndexOf("<tr");
                            int hh_row1To = sTblHH.IndexOf("/tr>", hh_row1From + 3) + 4;

                            int hh_row2From = sTblHH.IndexOf("<tr", hh_row1To);
                            int hh_row2To = sTblHH.IndexOf("/tr>", hh_row2From) + 6;
                            string hh_row2Str = sTblHH.Substring(hh_row1To, hh_row2To - hh_row2From);
                            string hh_row2 = hh_row2Str;

                            // find row contain sum phutung
                            if (sTblHH.IndexOf("{TongSL_PhuTung}") > -1 || sTblHH.IndexOf("{TongThue_PhuTung}") > -1 || sTblHH.IndexOf("{TongCK_PhuTung}") > -1 || sTblHH.IndexOf("{TongTienPhuTung_TruocCK}") > -1 || sTblHH.IndexOf("{TongTienPhuTung_TruocVAT}") > -1 || sTblHH.IndexOf("{TongTienPhuTung}") > -1)
                            {
                                int hh_lastRowFrom = sTblHH.LastIndexOf("<tr", sTblHH.IndexOf("</tbody"));
                                int hh_lastRowTo = sTblHH.LastIndexOf("</tr", sTblHH.IndexOf("</tbody")) + 5;
                                string hh_lastRowStr = sTblHH.Substring(hh_lastRowFrom, hh_lastRowTo - hh_lastRowFrom);
                                string hh_lastRow = hh_lastRowStr;
                                hh_lastRowStr = string.Concat(" <!-- ko if: $root.CTHoaDonPrint_VatTu().length - 1 == $index() --> ", hh_lastRowStr, " <!--/ko-->");
                                sTblHH = sTblHH.Replace(hh_lastRow, hh_lastRowStr);
                            }


                            hh_row2Str = string.Concat(" <!--ko foreach: $data.HangHoas --> ", hh_row2, " <!--/ko-->");
                            sTblHH = string.Concat("<!--ko foreach: $root.CTHoaDonPrint_VatTu --> ", sTblHH, " <!--/ko-->");
                            hh_sHeader = string.Concat(" <!-- ko if: $root.CTHoaDonPrint_VatTu().length > 0 --> ", hh_sHeader, " <!--/ko-->");

                            sTblHH = sTblHH.Replace(hh_row2, hh_row2Str);
                            sTblHH = Replace_TheoNhom(sTblHH);
                            sTblHH = Replace_ThongTinHangHoa(sTblHH);
                            sTblHH = sTblHH.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                            #endregion

                            #region tblDichVu
                            var startDV = content1.IndexOf("{TheoDichVu_Nhom}");
                            var opentblDV = content1.IndexOf("tbody", startDV) - 1;
                            var closeblDV = content1.IndexOf("tbody", opentblDV + 6);
                            var sTblDV = content1.Substring(opentblDV, closeblDV - opentblDV + 6);
                            var sTblDV_goc = sTblDV;

                            var dv_headerFrom = content1.IndexOf("thead", startDV) - 1;
                            var dv_headerTo = content1.IndexOf("thead", dv_headerFrom + 5);
                            var dv_sHeader = content1.Substring(dv_headerFrom, dv_headerTo - dv_headerFrom + 6);
                            var dv_sHeaderGoc = dv_sHeader;

                            int dv_row1From = sTblDV.IndexOf("<tr");
                            int dv_row1To = sTblDV.IndexOf("/tr>", hh_row1From + 3) + 4;

                            int dv_row2From = sTblDV.IndexOf("<tr", dv_row1To);
                            int dv_row2To = sTblDV.IndexOf("/tr>", dv_row2From) + 5;
                            string dv_row2Str = sTblDV.Substring(dv_row2From, dv_row2To - dv_row2From);
                            string dv_row2 = dv_row2Str;

                            // find row contain sum dichvu
                            if (sTblDV.IndexOf("{TongSL_DichVu}") > -1 || sTblDV.IndexOf("{TongThue_DichVu}") > -1 || sTblDV.IndexOf("{TongCK_DichVu}") > -1 || sTblDV.IndexOf("{TongTienDichVu_TruocCK}") > -1 || sTblDV.IndexOf("{TongTienDichVu_TruocVAT}") > -1 || sTblDV.IndexOf("{TongTienDichVu}") > -1)
                            {
                                int dv_lastRowFrom = sTblDV.LastIndexOf("<tr", sTblDV.IndexOf("</tbody"));
                                int dv_lastRowTo = sTblDV.LastIndexOf("</tr>", sTblDV.IndexOf("</tbody")) + 5;
                                string dv_lastRowStr = sTblDV.Substring(dv_lastRowFrom, dv_lastRowTo - dv_lastRowFrom);
                                string dv_lastRow = dv_lastRowStr;
                                dv_lastRowStr = string.Concat(" <!-- ko if: $root.CTHoaDonPrint_DichVu().length - 1 == $index() --> ", dv_lastRowStr, " <!--/ko-->");
                                sTblDV = sTblDV.Replace(dv_lastRow, dv_lastRowStr);
                            }

                            dv_row2Str = string.Concat(" <!--ko foreach: $data.HangHoas --> ", dv_row2, " <!--/ko-->");
                            sTblDV = string.Concat("<!--ko foreach: $root.CTHoaDonPrint_DichVu --> ", sTblDV, " <!--/ko-->");
                            dv_sHeader = string.Concat(" <!-- ko if: $root.CTHoaDonPrint_DichVu().length > 0 --> ", dv_sHeader, " <!--/ko-->");

                            sTblDV = sTblDV.Replace(dv_row2, dv_row2Str);
                            sTblDV = Replace_TheoNhom(sTblDV);
                            sTblDV = Replace_ThongTinHangHoa(sTblDV);
                            sTblDV = sTblDV.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                            #endregion

                            content1 = content1.Replace("{TheoHangHoa_Nhom}", "");
                            content1 = content1.Replace("{TheoDichVu_Nhom}", "");
                            content1 = content1.Replace(sTblHH_goc, sTblHH);
                            content1 = content1.Replace(sTblDV_goc, sTblDV);
                            content1 = content1.Replace(hh_sHeaderGoc, hh_sHeader);
                            content1 = content1.Replace(dv_sHeaderGoc, dv_sHeader);
                        }
                        else
                        {
                            if (content1.IndexOf("{Nhom_HangHoaDV}") > -1)
                            {
                                var startHH = content1.IndexOf("{Nhom_HangHoaDV}");
                                var openTbl = content1.IndexOf("tbody", startHH) - 1;
                                var closeTbl = content1.IndexOf("tbody", openTbl + 6);
                                var tbl = content1.Substring(openTbl, closeTbl - openTbl + 6);
                                var tbl_goc = tbl;

                                // tbl = string.Concat("<!--ko foreach: $root.CTHoaDon_TheoNhom_VaHHDV -->", tbl);
                                if (tbl_goc.IndexOf("{TenNhomHangHoa}") > -1)
                                {
                                    var row1From = tbl_goc.IndexOf("<tr", tbl_goc.IndexOf("tbody"));
                                    var row1To = tbl_goc.IndexOf("/tr>", row1From + 4) + 4;
                                    var row1 = tbl_goc.Substring(row1From, row1To - row1From);
                                    var row1_goc = row1;

                                    row1 = string.Concat("<!--ko foreach: {data: $root.CTHoaDon_TheoNhom_VaHHDV, as :'itNhom'} -->", row1);

                                    var row2From = tbl_goc.IndexOf("<tr", row1To);
                                    var row2To = tbl_goc.IndexOf("/tr>", row2From + 2) + 4;
                                    var row2 = tbl_goc.Substring(row2From, row2To - row2From);
                                    var row2_goc = row2;

                                    // find row 2: hanghoa or dichvu
                                    var strDV = "<!--ko foreach: {data: itNhom.ListDichVus.DichVus, as: 'itDV'} -->";
                                    var strHH = "<!--ko foreach: {data: itNhom.ListHangHoas.HangHoas, as: 'itHH'} -->";

                                    #region HangHoa - DichVu
                                    if (row2.IndexOf("{TheoHangHoa}") > -1)
                                    {
                                        row2 = row2.Replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                        row2 = string.Concat(strHH, row2);

                                        // row3: thongtin hang
                                        var row3From = tbl_goc.IndexOf("<tr", row2To);
                                        var row3To = tbl_goc.IndexOf("/tr>", row3From + 3) + 4;
                                        var row3 = tbl_goc.Substring(row3From, row3To - row3From);
                                        var row3_goc = row3;

                                        var row4From = row3To;
                                        var row4To = tbl_goc.IndexOf("/tr>", row4From + 2) + 4;
                                        var row4 = tbl_goc.Substring(row4From, row4To - row4From);
                                        var row4_goc = row4;

                                        if (row4.IndexOf("{NhomHH_ThanhToan") > -1
                                            || row4.IndexOf("{NhomHH_TruocCK") > -1
                                            || row4.IndexOf("{NhomHH_TruocVAT") > -1)
                                        {
                                            // hanghoa - cotong
                                            row4 = row4.Replace("<tr", " <tr data-bind=\"visible: itNhom.ListHangHoas.HangHoas.length === $index() + 1\"");
                                            row4 = string.Concat(row4, "<!--/ko -->");

                                            var row5From = row4To;
                                            var row5To = tbl_goc.IndexOf("/tr>", row5From + 2) + 5;
                                            var row5 = tbl_goc.Substring(row5From, row5To - row5From);
                                            var row5_goc = row5;

                                            if (row5.IndexOf("{TheoDichVu}") > -1)
                                            {
                                                row5 = row5.Replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                                row5 = string.Concat(strDV, row5);

                                                var row6From = row5To;
                                                var row6To = tbl_goc.IndexOf("/tr>", row6From + 2) + 5;
                                                var row6 = tbl_goc.Substring(row6From, row6To - row6From);
                                                var row6_goc = row6;

                                                var row7From = row6To;
                                                var row7To = tbl_goc.IndexOf("/tr>", row7From + 2) + 5;
                                                var row7 = tbl_goc.Substring(row7From, row7To - row7From);
                                                var row7_goc = row7;

                                                if (row7.IndexOf("{NhomDV_ThanhToan") > -1
                                                   || row7.IndexOf("{NhomDV_TruocCK") > -1
                                                   || row7.IndexOf("{NhomDV_TruocVAT") > -1)
                                                {
                                                    row7 = row7.Replace("<tr", " <tr data-bind=\"visible: itNhom.ListDichVus.DichVus.length === $index() + 1\"");

                                                    // tong hh + dv theo nhom
                                                    var row8From = row7To;
                                                    var row8To = tbl_goc.IndexOf("/tr>", row8From + 2) + 5;
                                                    if (row8To > 0)
                                                    {
                                                        var row8 = tbl_goc.Substring(row8From, row8To - row8From);
                                                        var row8_goc = row8;
                                                        if (row8.IndexOf("{TongTienTheoNhom") > -1)
                                                        {
                                                            row7 = string.Concat(row7, " <!--/ko -->");
                                                            row8 = string.Concat(row8, " <!--/ko -->");
                                                            tbl = tbl.Replace(row8_goc, row8);
                                                        }
                                                        else
                                                        {
                                                            row7 = string.Concat(row7, " <!--/ko --> <!--/ko -->");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        row7 = string.Concat(row7, " <!--/ko --> <!--/ko -->");
                                                    }
                                                    tbl = tbl.Replace(row7_goc, row7);
                                                }
                                                else
                                                {
                                                    row6 = string.Concat(row6, " <!--/ko --> <!--/ko --> ");
                                                    tbl = tbl.Replace(row6_goc, row6);
                                                }
                                                tbl = tbl.Replace(row5_goc, row5);
                                                tbl = tbl.Replace(row4_goc, row4);
                                            }
                                            else
                                            {
                                                // err
                                            }
                                            tbl = tbl.Replace(row2_goc, row2);
                                        }
                                        else
                                        {
                                            // khong tong HangHoa (todo)
                                        }
                                    }
                                    #endregion
                                    #region DichVu- HangHoa
                                    else
                                    {
                                        if (row2.IndexOf("{TheoDichVu}") > -1)
                                        {
                                            row2 = row2.Replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                            row2 = string.Concat(strDV, row2);

                                            // row3: thongtin hang
                                            var row3From = tbl_goc.IndexOf("<tr", row2To);
                                            var row3To = tbl_goc.IndexOf("/tr>", row3From + 3) + 4;
                                            var row3 = tbl_goc.Substring(row3From, row3To - row3From);
                                            var row3_goc = row3;

                                            var row4From = row3To;
                                            var row4To = tbl_goc.IndexOf("/tr>", row4From + 2) + 4;
                                            var row4 = tbl_goc.Substring(row4From, row4To - row4From);
                                            var row4_goc = row4;

                                            if (row4.IndexOf("{NhomDV_ThanhToan") > -1
                                                || row4.IndexOf("{NhomDV_TruocCK") > -1
                                                || row4.IndexOf("{NhomDV_TruocVAT") > -1)
                                            {
                                                // hanghoa - cotong
                                                row4 = row4.Replace("<tr", " <tr data-bind=\"visible: itNhom.ListDichVus.DichVus.length === $index() + 1\"");
                                                row4 = string.Concat(row4, "<!--/ko -->");

                                                var row5From = row4To;
                                                var row5To = tbl_goc.IndexOf("/tr>", row5From + 2) + 5;
                                                var row5 = tbl_goc.Substring(row5From, row5To - row5From);
                                                var row5_goc = row5;

                                                if (row5.IndexOf("{TheoHangHoa}") > -1)
                                                {
                                                    row5 = row5.Replace("<tr", " <tr data-bind=\"visible: $index()===0\"");
                                                    row5 = string.Concat(strHH, row5);

                                                    var row6From = row5To;
                                                    var row6To = tbl_goc.IndexOf("/tr>", row6From + 2) + 5;
                                                    var row6 = tbl_goc.Substring(row6From, row6To - row6From);
                                                    var row6_goc = row6;

                                                    var row7From = row6To;
                                                    var row7To = tbl_goc.IndexOf("/tr>", row7From + 2) + 5;
                                                    var row7 = tbl_goc.Substring(row7From, row7To - row7From);
                                                    var row7_goc = row7;

                                                    if (row7.IndexOf("{NhomHH_ThanhToan") > -1
                                                       || row7.IndexOf("{NhomHH_TruocCK") > -1
                                                       || row7.IndexOf("{NhomHH_TruocVAT") > -1)
                                                    {
                                                        row7 = row7.Replace("<tr", " <tr data-bind=\"visible: itNhom.ListHangHoas.HangHoas.length === $index() + 1\"");

                                                        // tong hh + dv theo nhom
                                                        var row8From = row7To;
                                                        var row8To = tbl_goc.IndexOf("/tr>", row8From + 2) + 5;
                                                        if (row8To > 0)
                                                        {
                                                            var row8 = tbl_goc.Substring(row8From, row8To - row8From);
                                                            var row8_goc = row8;
                                                            if (row8.IndexOf("{TongTienTheoNhom") > -1)
                                                            {
                                                                row7 = string.Concat(row7, " <!--/ko -->");
                                                                row8 = string.Concat(row8, " <!--/ko -->");
                                                                tbl = tbl.Replace(row8_goc, row8);
                                                            }
                                                            else
                                                            {
                                                                row7 = string.Concat(row7, " <!--/ko --> <!--/ko -->");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            row7 = string.Concat(row7, " <!--/ko --> <!--/ko -->");
                                                        }
                                                        tbl = tbl.Replace(row7_goc, row7);
                                                    }
                                                    else
                                                    {
                                                        row6 = string.Concat(row6, " <!--/ko --> <!--/ko --> ");
                                                        tbl = tbl.Replace(row6_goc, row6);
                                                    }
                                                    tbl = tbl.Replace(row5_goc, row5);
                                                    tbl = tbl.Replace(row4_goc, row4);
                                                }
                                                else
                                                {
                                                    // err
                                                }
                                                tbl = tbl.Replace(row2_goc, row2);
                                            }
                                        }
                                        else
                                        {
                                            // err
                                        }
                                    }
                                    #endregion

                                    tbl = tbl.Replace(row1_goc, row1);
                                    tbl = tbl.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                                    content1 = content1.Replace(tbl_goc, tbl);
                                    content1 = Replace_ThongTinHangHoa(content1);
                                }
                                else
                                {
                                    // err
                                }
                                content1 = content1.Replace("{Nhom_HangHoaDV}", "");
                                content1 = content1.Replace("{TheoHangHoa}", "");
                                content1 = content1.Replace("{TheoDichVu}", "");
                                content1 = content1.Replace("{NhomDV_TruocCK}", "<span data-bind=\"text: formatNumber(itNhom.ListDichVus.NhomDV_TruocCK)\"></span>");
                                content1 = content1.Replace("{NhomDV_TruocVAT}", "<span data-bind=\"text: formatNumber(itNhom.ListDichVus.NhomDV_TruocVAT)\"></span>");
                                content1 = content1.Replace("{NhomDV_ThanhToan}", "<span data-bind=\"text: formatNumber(itNhom.ListDichVus.NhomDV_ThanhToan)\"></span>");
                                content1 = content1.Replace("{NhomHH_TruocCK}", "<span data-bind=\"text: formatNumber(itNhom.ListHangHoas.NhomHH_TruocCK)\"></span>");
                                content1 = content1.Replace("{NhomHH_TruocVAT}", "<span data-bind=\"text: formatNumber(itNhom.ListHangHoas.NhomHH_TruocVAT)\"></span>");
                                content1 = content1.Replace("{NhomHH_ThanhToan}", "<span data-bind=\"text: formatNumber(itNhom.ListHangHoas.NhomHH_ThanhToan)\"></span>");

                                content1 = Replace_TheoNhom(content1);
                            }
                            else
                            {
                                if (content1.IndexOf("{TenHangHoa") != -1)
                                {
                                    var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa")) - 1;
                                    var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa")) + 6;
                                    var temptable = content1.Substring(open, close - open);
                                    var temptable1 = temptable;

                                    int row1From = temptable.IndexOf("<tr");
                                    int row1To = temptable.IndexOf("/tr>") - 4;
                                    string row1Str = temptable.Substring(row1From, row1To);
                                    string row1 = row1Str;

                                    int nextRowFrom = row1To;

                                    row1Str = string.Concat(" <!--ko foreach: $data.CTHoaDonPrint --> ", row1Str);
                                    if (row1Str.IndexOf("{SoLuong") > -1 || row1Str.IndexOf("{ThanhTien") > -1)
                                    {
                                        int lastRowFrom = row1To;
                                        int lastRowTo = temptable.IndexOf("<tr", lastRowFrom + 1);
                                        if (lastRowTo < 0)
                                        {
                                            lastRowTo = temptable.LastIndexOf("/tr>") + 5;
                                        }
                                        string lastRowStr = temptable.Substring(lastRowFrom, lastRowTo - lastRowFrom);
                                        string lastRow = lastRowStr;
                                        if (lastRowStr.IndexOf("{GhiChu_NVThucHien}") > -1 || lastRowStr.IndexOf("{GhiChu_NVTuVan}") > -1)
                                        {
                                            lastRowStr = string.Concat(lastRowStr, "<!--/ko-->");
                                            temptable = temptable.Replace(lastRow, lastRowStr);
                                        }
                                        else
                                        {
                                            row1Str = string.Concat(row1Str, "<!--/ko-->");
                                        }
                                        goto ReplaceDetail;
                                    }
                                    else
                                    {
                                        goto CheckRowNext;
                                    }
                                CheckRowNext:
                                    {
                                        int nextRowTo = temptable.IndexOf("<tr", nextRowFrom + 1);
                                        if (nextRowTo < 0)
                                        {
                                            nextRowTo = temptable.LastIndexOf("/tr>") + 5;
                                        }
                                        string nextRowStr = temptable.Substring(nextRowFrom, nextRowTo - nextRowFrom);
                                        string nextRow = nextRowStr;

                                        if (nextRowStr.IndexOf("{SoLuong") > -1 || nextRowStr.IndexOf("{ThanhTien") > -1)
                                        {
                                            int lastRowFrom = nextRowTo;
                                            int lastRowTo = temptable.IndexOf("<tr", lastRowFrom + 1);
                                            if (lastRowTo < 0)
                                            {
                                                lastRowTo = temptable.LastIndexOf("/tr>") + 5;
                                            }
                                            string lastRowStr = temptable.Substring(lastRowFrom, lastRowTo - lastRowFrom);
                                            string lastRow = lastRowStr;
                                            if (lastRowStr.IndexOf("{GhiChu_NVThucHien}") > -1 || lastRowStr.IndexOf("{GhiChu_NVTuVan}") > -1)
                                            {
                                                lastRowStr = string.Concat(lastRowStr, "<!--/ko-->");
                                                temptable = temptable.Replace(lastRow, lastRowStr);
                                            }
                                            else
                                            {
                                                nextRowStr = string.Concat(nextRowStr, "<!--/ko-->");
                                                temptable = temptable.Replace(nextRow, nextRowStr);
                                            }
                                            goto ReplaceDetail;
                                        }
                                        else
                                        {
                                            nextRowFrom = nextRowTo;
                                            goto CheckRowNext;
                                        }
                                    }

                                ReplaceDetail:
                                    {
                                        temptable = temptable.Replace(row1, row1Str);
                                        temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                                        temptable = Replace_ThongTinHangHoa(temptable);

                                        content1 = content1.Replace(temptable1, temptable);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            content1 = Replace_HangMucSC(content1);
            content1 = Replace_VatDungKemTheo(content1);
            content1 = Replace_ThongTinChung(content1);

            content1 = content1.Replace("{MaHoaDonTraHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDonTraHang\"></span>");
            content1 = content1.Replace("{MaKhachHang}", "<span data-bind=\"text: InforHDprintf().MaDoiTuong\"></span>");
            content1 = content1.Replace("{TenKhachHang}", "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
            content1 = content1.Replace("{TenNhaCungCap}", "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
            content1 = content1.Replace("{NgaySinhKH}", "<span data-bind=\"text: InforHDprintf().NgaySinh_NgayTLap\"></span>");
            content1 = content1.Replace("{DiaChi}", "<span data-bind=\"text: InforHDprintf().DiaChiKhachHang\"></span>");
            content1 = content1.Replace("{DienThoai}", "<span data-bind=\"text: InforHDprintf().DienThoaiKhachHang\"></span>");
            content1 = content1.Replace("{MaSoThue}", "<span data-bind=\"text: InforHDprintf().MaSoThue\"></span>");
            content1 = content1.Replace("{TaiKhoanNganHang}", "<span data-bind=\"text: $root.InforHDprintf().TaiKhoanNganHang\"></span>");
            content1 = content1.Replace("{TongDiemKhachHang}", "<span data-bind=\"text: InforHDprintf().TongTichDiem\"></span>");
            content1 = content1.Replace("{NhanVienBanHang}", "<span data-bind=\"text: InforHDprintf().NhanVienBanHang\"></span>");
            content1 = content1.Replace("{TenPhongBan}", "<span data-bind=\"text: InforHDprintf().TenPhongBan\"></span>");
            content1 = content1.Replace("{NguoiTao}", "<span data-bind=\"text: $root.InforHDprintf().NguoiTaoHD\"></span>");
            content1 = content1.Replace("{KhachCanTra}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().PhaiThanhToan,2)\"></span>");

            content1 = content1.Replace("{DienGiai}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: $root.InforHDprintf().DienGiai\"></span>");
            content1 = content1.Replace("{TongTienHang}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienHang,2)\"></span>");
            content1 = content1.Replace("{TongTienHDSauGiamGia}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienHDSauGiamGia,2)\"></span>");
            content1 = content1.Replace("{TongTienHDSauVAT}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienHDSauVAT,2)\"></span>");
            content1 = content1.Replace("{DaThanhToan}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().DaThanhToan,2)\"></span>");
            content1 = content1.Replace("{ChietKhauHoaDon}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongGiamGia,2)\"></span>");
            content1 = content1.Replace("{DiaChiCuaHang}", "<span data-bind=\"text: InforHDprintf().DiaChiCuaHang\"></span>");
            content1 = content1.Replace("{PhiTraHang}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongChiPhiHangTra,2)\"></span>");
            content1 = content1.Replace("{TongChiPhi}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongChiPhi,2)\"></span>");// tongchiphi hangmua
            content1 = content1.Replace("{ChiPhi}", "<span data-bind=\"text:formatNumber($root.InforHDprintf().ChiPhi)\"></span>");// tongchiphi cuahang tra cho ben thu 3
            content1 = content1.Replace("{ChiPhi_GhiChu}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: $root.InforHDprintf().ChiPhi_GhiChu\"></span>");
            content1 = content1.Replace("{TongTienThue}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienThue)\"></span>");
            content1 = content1.Replace("{TongThueKhachHang}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongThueKhachHang)\"></span>");

            content1 = content1.Replace("{TongTienTraHang}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienTraHang)\"></span>");
            content1 = content1.Replace("{TongTienTra}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienTra)\"></span>");
            content1 = content1.Replace("{TongCong}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongCong,2)\"></span>");
            content1 = content1.Replace("{TongSoLuongHang}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongSoLuongHang)\"></span>");
            content1 = content1.Replace("{ChiPhiNhap}", "<span data-bind=\"text: $root.InforHDprintf().ChiPhiNhap\"></span>");
            content1 = content1.Replace("{NoTruoc}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().NoTruoc)\"></span>");
            content1 = content1.Replace("{NoSau}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().NoSau)\"></span>");

            content1 = content1.Replace("{DiemGiaoDich}", "<span data-bind=\"text: $root.InforHDprintf().DiemGiaoDich\"></span>");
            content1 = content1.Replace("{TongGiamGiaHang}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongGiamGiaHang,2)\"></span>");
            content1 = content1.Replace("{TongTienHangChuaChietKhau}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienHangChuaCK)\"></span>");
            content1 = content1.Replace("{TienPOS}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TienATM,2)\"></span>");
            content1 = content1.Replace("{TienMat}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TienMat,2)\"></span>");// vì TienATM = TienGui (store), nhưng lại bị gán lại = TienChuyenKhoan khi in, nên gtrị bị sai ==> get trong này luôn 
            content1 = content1.Replace("{TraLaiTienDatCoc}", "<span data-bind=\"text: formatNumber(InforHDprintf().TraLaiTienDatCoc)\"></span>");
            content1 = content1.Replace("{TTBangTienCoc}", "<span data-bind=\"text: formatNumber(InforHDprintf().TTBangTienCoc,2)\"></span>");
            content1 = content1.Replace("{TienChuyenKhoan}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().ChuyenKhoan,2)\"></span>");
            content1 = content1.Replace("{TienDoiDiem}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TienDoiDiem,2)\"></span>");
            content1 = content1.Replace("{TienTheGiaTri}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TienTheGiaTri,2)\"></span>");
            content1 = content1.Replace("{PTChietKhauHD}", "<span data-bind=\"text: $root.InforHDprintf().TongChietKhau\"></span>");
            content1 = content1.Replace("{TongGiamGiaHD_HH}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongGiamGiaHD_HH,2)\"></span>");
            content1 = content1.Replace("{ChietKhauNVHoaDon}", "<span data-bind=\"text: $root.InforHDprintf().ChietKhauNVHoaDon\"></span>");
            content1 = content1.Replace("{ChietKhauNVHoaDon_InGtriCK}", "<span data-bind=\"text: InforHDprintf().ChietKhauNVHoaDon_InGtriCK\"></span>");

            content1 = content1.Replace("{TienThuaTraKhach}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TienThua)\"></span>");
            content1 = content1.Replace("{TienKhachThieu}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TienKhachThieu,2)\"></span>");
            content1 = content1.Replace("{BH_TienThua}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().BH_TienThua)\"></span>");
            content1 = content1.Replace("{BH_ConThieu}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().BH_ConThieu)\"></span>");
            content1 = content1.Replace("{HD_TienThua}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().HD_TienThua)\"></span>");
            content1 = content1.Replace("{HD_ConThieu}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().HD_ConThieu)\"></span>");
            content1 = content1.Replace("{BH_TienBangChu}", "<span data-bind=\"text: $root.InforHDprintf().BH_TienBangChu\"></span>");
            content1 = content1.Replace("{KH_TienBangChu}", "<span data-bind=\"text: $root.InforHDprintf().KH_TienBangChu\"></span>");
            content1 = content1.Replace("{BH_NoTruoc}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().BH_NoTruoc)\"></span>");
            content1 = content1.Replace("{BH_NoSau}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().BH_NoSau)\"></span>");

            #region ChuyenHang
            content1 = content1.Replace("{ChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().ChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{NguoiChuyen}", "<span data-bind=\"text: InforHDprintf().NguoiChuyen\"></span>");
            content1 = content1.Replace("{ChiNhanhNhan}", "<span data-bind=\"text: InforHDprintf().ChiNhanhNhan\"></span>");
            content1 = content1.Replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
            content1 = content1.Replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{TongSoLuongChuyen}", "<span data-bind=\"text: InforHDprintf().TongSoLuongChuyen\"></span>");
            content1 = content1.Replace("{TongSoLuongNhan}", "<span data-bind=\"text: InforHDprintf().TongSoLuongNhan\"></span>");
            content1 = content1.Replace("{TongTienChuyen}", "<span data-bind=\"text: InforHDprintf().TongTienChuyen\"></span>");
            content1 = content1.Replace("{TongTienNhan}", "<span data-bind=\"text: InforHDprintf().TongTienNhan\"></span>");
            #endregion

            #region Phieu ThuChi
            content1 = content1.Replace("{MaPhieu}", "<span data-bind=\"text: InforHDprintf().MaPhieu\"></span>");
            content1 = content1.Replace("{NguoiNopTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
            content1 = content1.Replace("{NguoiNhanTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
            content1 = content1.Replace("{GiaTriPhieu}", "<span data-bind=\"text: InforHDprintf().GiaTriPhieu\"></span>");
            content1 = content1.Replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
            content1 = content1.Replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{NoiDungThu}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: InforHDprintf().NoiDungThu\"></span>");
            content1 = content1.Replace("{TienBangChu}", "<span data-bind=\"text: $root.InforHDprintf().TienBangChu\"></span>");
            content1 = content1.Replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{ChiNhanhBanHang}", "<span data-bind=\"text: InforHDprintf().ChiNhanhBanHang\"></span>");
            content1 = content1.Replace("{HoaDonLienQuan}", "<span data-bind=\"text: InforHDprintf().HoaDonLienQuan\"></span>");
            content1 = content1.Replace("{PhuongThucTT}", "<span data-bind=\"text: InforHDprintf().PhuongThucTT\"></span>");
            content1 = content1.Replace("{KhoanMucThuChi}", "<span data-bind=\"text: InforHDprintf().KhoanMucThuChi\"></span>");
            #endregion
            // Kiểm kho
            content1 = content1.Replace("{NguoiCanBang}", "<span data-bind=\"text: InforHDprintf().NguoiCanBang\"></span>");
            content1 = content1.Replace("{TrangThaiKK}", "<span data-bind=\"text: InforHDprintf().TrangThaiKK\"></span>");

            content1 = content1.Replace("{NgayCanBang}", "<span data-bind=\"text: InforHDprintf().NgayCanBang\"></span>");
            content1 = content1.Replace("{TongThucTe}", "<span data-bind=\"text: formatNumber(InforHDprintf().TongThucTe)\"></span>");
            content1 = content1.Replace("{TongLechTang}", "<span data-bind=\"text: formatNumber(InforHDprintf().TongLechTang)\"></span>");
            content1 = content1.Replace("{TongLechGiam}", "<span data-bind=\"text: formatNumber(InforHDprintf().TongLechGiam)\"></span>");
            content1 = content1.Replace("{TongChenhLech}", "<span data-bind=\"text: formatNumber(InforHDprintf().TongChenhLech)\"></span>");

            // the gia tri
            content1 = content1.Replace("{SoDuDatCoc}", "<span data-bind=\"text: InforHDprintf().SoDuDatCoc\"></span>");
            content1 = content1.Replace("{TongTaiKhoanThe}", "<span data-bind=\"text: InforHDprintf().TongTaiKhoanThe\"></span>");
            content1 = content1.Replace("{TongSuDungThe}", "<span data-bind=\"text: InforHDprintf().TongSuDungThe\"></span>");
            content1 = content1.Replace("{SoDuConLai}", "<span data-bind=\"text: InforHDprintf().SoDuConLai\"></span>");

            content1 = content1.Replace("{Ngay}", "<span data-bind=\"text: InforHDprintf().Ngay\"></span>");
            content1 = content1.Replace("{Thang}", "<span data-bind=\"text: InforHDprintf().Thang\"></span>");
            content1 = content1.Replace("{Nam}", "<span data-bind=\"text: InforHDprintf().Nam\"></span>");

            #region Gara
            content1 = content1.Replace("{CoVanDichVu}", "<span data-bind=\"text: InforHDprintf().CoVanDichVu\"></span>");
            content1 = content1.Replace("{NhanVienTiepNhan}", "<span data-bind=\"text: InforHDprintf().NhanVienTiepNhan\"></span>");
            content1 = content1.Replace("{MaPhieuTiepNhan}", "<span data-bind=\"text: InforHDprintf().MaPhieuTiepNhan\"></span>");
            content1 = content1.Replace("{BienSo}", "<span data-bind=\"text: InforHDprintf().BienSo\"></span>");
            content1 = content1.Replace("{NgayVaoXuong}", "<span data-bind=\"text: InforHDprintf().NgayVaoXuong\"></span>");
            content1 = content1.Replace("{NgayHoanThanhDuKien}", "<span data-bind=\"text: InforHDprintf().NgayXuatXuongDuKien\"></span>");
            content1 = content1.Replace("{SoMay}", "<span data-bind=\"text: InforHDprintf().SoMay\"></span>");
            content1 = content1.Replace("{SoKhung}", "<span data-bind=\"text: InforHDprintf().SoKhung\"></span>");
            content1 = content1.Replace("{SoKmVao}", "<span data-bind=\"text: InforHDprintf().SoKmVao\"></span>");
            content1 = content1.Replace("{SoKmRa}", "<span data-bind=\"text: InforHDprintf().SoKmRa\"></span>");
            content1 = content1.Replace("{NgayXuatXuong}", "<span data-bind=\"text: InforHDprintf().NgayXuatXuong\"></span>");
            content1 = content1.Replace("{TenMauXe}", "<span data-bind=\"text: InforHDprintf().TenMauXe\"></span>");
            content1 = content1.Replace("{TenLoaiXe}", "<span data-bind=\"text: InforHDprintf().TenLoaiXe\"></span>");
            content1 = content1.Replace("{TenHangXe}", "<span data-bind=\"text: InforHDprintf().TenHangXe\"></span>");
            content1 = content1.Replace("{MauSon}", "<span data-bind=\"text: InforHDprintf().MauSon\"></span>");
            content1 = content1.Replace("{DungTich}", "<span data-bind=\"text: InforHDprintf().DungTich\"></span>");
            content1 = content1.Replace("{NamSanXuat}", "<span data-bind=\"text: InforHDprintf().NamSanXuat\"></span>");
            content1 = content1.Replace("{HopSo}", "<span data-bind=\"text: InforHDprintf().HopSo\"></span>");

            content1 = content1.Replace("{TongSL_DichVu}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongSL_DichVu)\"></span>");
            content1 = content1.Replace("{TongThue_DichVu}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongThue_DichVu,2)\"></span>");
            content1 = content1.Replace("{TongCK_DichVu}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongCK_DichVu,2)\"></span>");
            content1 = content1.Replace("{TongTienDichVu}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienDichVu,2)\"></span>");
            content1 = content1.Replace("{TongTienDichVu_TruocVAT}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienDichVu_TruocVAT,2)\"></span>");
            content1 = content1.Replace("{TongTienDichVu_TruocCK}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienDichVu_TruocCK,2)\"></span>");

            content1 = content1.Replace("{TongSL_PhuTung}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongSL_PhuTung)\"></span>");
            content1 = content1.Replace("{TongThue_PhuTung}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongThue_PhuTung,2)\"></span>");
            content1 = content1.Replace("{TongCK_PhuTung}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongCK_PhuTung,2)\"></span>");
            content1 = content1.Replace("{TongTienPhuTung}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienPhuTung,2)\"></span>");
            content1 = content1.Replace("{TongTienPhuTung_TruocVAT}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienPhuTung_TruocVAT,2)\"></span>");
            content1 = content1.Replace("{TongTienPhuTung_TruocCK}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongTienPhuTung_TruocCK,2)\"></span>");
            content1 = content1.Replace("{TongThanhToan}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().TongThanhToan,2)\"></span>");
            content1 = content1.Replace("{PhaiThanhToanBaoHiem}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().PhaiThanhToanBaoHiem,2)\"></span>");

            content1 = content1.Replace("{LH_Ten}", "<span data-bind=\"text: InforHDprintf().TenLienHe\"></span>");
            content1 = content1.Replace("{LH_SDT}", "<span data-bind=\"text: InforHDprintf().SoDienThoaiLienHe\"></span>");

            content1 = content1.Replace("{TongKhach_BHThanhToan}", "<span data-bind=\"text: $root.InforHDprintf().TongKhach_BHThanhToan\"></span>");
            content1 = content1.Replace("{BaoHiemDaTra}", "<span data-bind=\"text: formatNumber($root.InforHDprintf().BaoHiemDaTra)\"></span>");
            content1 = content1.Replace("{TenBaoHiem}", "<span data-bind=\"text: InforHDprintf().TenBaoHiem\"></span>");
            content1 = content1.Replace("{BH_SDT}", "<span data-bind=\"text: InforHDprintf().BH_SDT\"></span>");
            content1 = content1.Replace("{BH_Email}", "<span data-bind=\"text: InforHDprintf().BH_Email\"></span>");
            content1 = content1.Replace("{BH_DiaChi}", "<span data-bind=\"text: InforHDprintf().BH_DiaChi\"></span>");
            content1 = content1.Replace("{BH_TenLienHe}", "<span data-bind=\"text: InforHDprintf().BH_TenLienHe\"></span>");
            content1 = content1.Replace("{BH_SDTLienHe}", "<span data-bind=\"text: InforHDprintf().BH_SDTLienHe\"></span>");

            content1 = content1.Replace("{SoVuBaoHiem}", "<span data-bind=\"text: $root.InforHDprintf().SoVuBaoHiem\"></span>");
            content1 = content1.Replace("{KhauTruTheoVu}", "<span data-bind=\"text: $root.InforHDprintf().KhauTruTheoVu\"></span>");
            content1 = content1.Replace("{PTGiamTruBoiThuong}", "<span data-bind=\"text: $root.InforHDprintf().PTGiamTruBoiThuong\"></span>");
            content1 = content1.Replace("{GiamTruBoiThuong}", "<span data-bind=\"text: $root.InforHDprintf().GiamTruBoiThuong\"></span>");
            content1 = content1.Replace("{TongTienThueBaoHiem}", "<span data-bind=\"text: $root.InforHDprintf().TongTienThueBaoHiem\"></span>");
            content1 = content1.Replace("{PTThueBaoHiem}", "<span data-bind=\"text: $root.InforHDprintf().PTThueBaoHiem\"></span>");
            content1 = content1.Replace("{PTThueHoaDon}", "<span data-bind=\"text: $root.InforHDprintf().PTThueHoaDon\"></span>");
            content1 = content1.Replace("{BHThanhToanTruocThue}", "<span data-bind=\"text: $root.InforHDprintf().BHThanhToanTruocThue\"></span>");
            content1 = content1.Replace("{TongTienBHDuyet}", "<span data-bind=\"text: $root.InforHDprintf().TongTienBHDuyet\"></span>");

            content1 = content1.Replace("{ChuXe}", "<span data-bind=\"text: InforHDprintf().ChuXe\"></span>");
            content1 = content1.Replace("{ChuXe_SDT}", "<span data-bind=\"text: InforHDprintf().ChuXe_SDT\"></span>");
            content1 = content1.Replace("{ChuXe_DiaChi}", "<span data-bind=\"text: InforHDprintf().ChuXe_DiaChi\"></span>");
            content1 = content1.Replace("{ChuXe_Email}", "<span data-bind=\"text: InforHDprintf().ChuXe_Email\"></span>");

            content1 = content1.Replace("{CoVan_SDT}", "<span data-bind=\"text: InforHDprintf().CoVan_SDT\"></span>");
            content1 = content1.Replace("{PTN_GhiChu}", "<span style=\"white-space:pre-wrap\"  data-bind=\"text: InforHDprintf().PTN_GhiChu\"></span>");
            #endregion
            return content1;
        }

        /// <summary>
        /// Hàm replace Các tham số để bind dữ liệu khi in (in nhiều hóa đơn)
        /// </summary>
        /// <param name="content1"></param>
        /// <returns></returns>
        private string ReaplaceMauIn_Multiple(string content1, string machungtu = "")
        {
            if (content1.IndexOf("{TheoNhomHang}") > -1 || content1.IndexOf("{Combo}") > -1
                || content1.IndexOf("{TheoHangHoa}") > -1 || content1.IndexOf("{TheoDichVu}") > -1)
            {
                // get temp print from .txt
                content1 = GetFileMauIn(commonEnum.DanhSachMauInA4.FirstOrDefault(o => o.Key.Equals(machungtu)).Value);
            }

            if (content1.IndexOf("{TenHangHoaMoi}") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa}")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa}")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;

                temptable = temptable.Replace("tbody", "tbody data-bind=\"foreach: BH_HoaDon_ChiTiet\"");
                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");

                temptable = Replace_ThongTinHangHoa(temptable);
                content1 = content1.Replace(temptable1, temptable);

                var openTbl2 = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) - 1;
                var closeTbl2 = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) + 6;
                var temptable2 = content1.Substring(openTbl2, closeTbl2 - openTbl2);
                var temptableMH = temptable2;

                temptable2 = temptable2.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrintMH\"");
                temptable2 = temptable2.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable2 = Replace_ThongTinHangHoa(temptable2);

                content1 = content1.Replace(temptableMH, temptable2);
            }
            else if (content1.IndexOf("{TenHangHoa}") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa}")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa}")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;
                temptable = temptable.Replace("tbody", "tbody data-bind=\"foreach: BH_HoaDon_ChiTiet\"");
                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable = Replace_ThongTinHangHoa(temptable);
                content1 = content1.Replace(temptable1, temptable);
            }

            content1 = "<style> .pageBreak {page-break-before:always;}</style > <div data-bind=\"foreach: ListHoaDon_ChiTietHoaDonPrint\" >" + content1;

            content1 = content1.Replace("{TenCuaHang}", "<span data-bind=\"text: TenCuaHang\"></span>");
            content1 = content1.Replace("{DienThoaiCuaHang}", "<span data-bind=\"text: DienThoaiCuaHang\"></span>");
            content1 = content1.Replace("{DiaChiCuaHang}", "<span data-bind=\"text: DiaChiCuaHang\"></span>");
            content1 = content1.Replace("{TenChiNhanh}", "<span data-bind=\"text: TenChiNhanh\"></span>");
            content1 = content1.Replace("{DienThoaiChiNhanh}", "<span data-bind=\"text: DienThoaiChiNhanh\"></span>");
            content1 = content1.Replace("{DiaChiChiNhanh}", "<span data-bind=\"text: DiaChiChiNhanh\"></span>");
            content1 = content1.Replace("{Logo}", "<img data-bind=\"attr: {src: LogoCuaHang}\" style=\"width:100%\" />");

            content1 = content1.Replace("{NgayBan}", "<span data-bind=\"text: NgayLapHoaDon\"></span>");
            content1 = content1.Replace("{NgayLapHoaDon}", "<span data-bind=\"text: NgayLapHoaDon\"></span>");
            content1 = content1.Replace("{NgayApDungGoiDV}", "<span data-bind=\"text: NgayApDungGoiDV\"></span>");
            content1 = content1.Replace("{HanSuDungGoiDV}", "<span data-bind=\"text: HanSuDungGoiDV\"></span>");
            content1 = content1.Replace("{MaHoaDon}", "<span data-bind=\"text: MaHoaDon\"></span>");
            content1 = content1.Replace("{MaHoaDonTraHang}", "<span data-bind=\"text: MaHoaDonTraHang\"></span>");
            content1 = content1.Replace("{MaKhachHang}", "<span data-bind=\"text: MaDoiTuong\"></span>");
            content1 = content1.Replace("{TenKhachHang}", "<span data-bind=\"text: TenDoiTuong\"></span>");
            content1 = content1.Replace("{TenNhaCungCap}", "<span data-bind=\"text: TenDoiTuong\"></span>");
            content1 = content1.Replace("{DiaChi}", "<span data-bind=\"text: DiaChiKhachHang\"></span>");
            content1 = content1.Replace("{DienThoai}", "<span data-bind=\"text: DienThoaiKhachHang\"></span>");
            content1 = content1.Replace("{TongDiemKhachHang}", "<span data-bind=\"text: TongTichDiem\"></span>");
            content1 = content1.Replace("{NhanVienBanHang}", "<span data-bind=\"text: NhanVienBanHang\"></span>");
            content1 = content1.Replace("{TenPhongBan}", "<span data-bind=\"text: TenPhongBan\"></span>");
            content1 = content1.Replace("{NguoiTao}", "<span data-bind=\"text: NguoiTaoHD\"></span>");
            content1 = content1.Replace("{NgayTao}", "<span data-bind=\"text: NgayTao\"></span>");
            content1 = content1.Replace("{Ngay}", "<span data-bind=\"text: Ngay\"></span>");
            content1 = content1.Replace("{Thang}", "<span data-bind=\"text: Thang\"></span>");
            content1 = content1.Replace("{Nam}", "<span data-bind=\"text: Nam\"></span>");

            content1 = content1.Replace("{TongTaiKhoanThe}", "<span data-bind=\"text: formatNumber(TongTaiKhoanThe,2)\"></span>");
            content1 = content1.Replace("{TongSuDungThe}", "<span data-bind=\"text: formatNumber(TongSuDungThe,2)\"></span>");
            content1 = content1.Replace("{SoDuConLai}", "<span data-bind=\"text: formatNumber(SoDuConLai,2)\"></span>");

            content1 = content1.Replace("{DienGiai}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: DienGiai\"></span>");
            content1 = content1.Replace("{TongTienHang}", "<span data-bind=\"text: formatNumber(TongTienHang,2)\"></span>");
            content1 = content1.Replace("{TongTienHDSauGiamGia}", "<span data-bind=\"text: formatNumber(TongTienHDSauGiamGia)\"></span>");
            content1 = content1.Replace("{DaThanhToan}", "<span data-bind=\"text: formatNumber(DaThanhToan,2)\"></span>");
            content1 = content1.Replace("{ChietKhauHoaDon}", "<span data-bind=\"text: formatNumber(TongGiamGia,2)\"></span>");
            content1 = content1.Replace("{PhiTraHang}", "<span data-bind=\"text: TongChiPhiHangTra\"></span>");

            content1 = content1.Replace("{TongTienHoaDonMua}", "<span data-bind=\"text: formatNumber(TongTienHoaDonMua,2)\"></span>");
            content1 = content1.Replace("{TienTraKhach}", "<span data-bind=\"text: formatNumber(PhaiTraKhach,2)\"></span>");
            content1 = content1.Replace("{KhachCanTra}", "<span data-bind=\"text: formatNumber(PhaiThanhToan)\"></span>");
            content1 = content1.Replace("{TongTienTraHang}", "<span data-bind=\"text: TongTienTraHang\"></span>");
            content1 = content1.Replace("{TongTienTra}", "<span data-bind=\"text: formatNumber(TongTienTra,2)\"></span>");
            content1 = content1.Replace("{TongCong}", "<span data-bind=\"text: formatNumber(TongCong,2)\"></span>");
            content1 = content1.Replace("{TongSoLuongHang}", "<span data-bind=\"text: formatNumber(TongSoLuongHang,2)\"></span>");
            content1 = content1.Replace("{ChiPhiNhap}", "<span data-bind=\"text: ChiPhiNhap\"></span>");
            content1 = content1.Replace("{NoTruoc}", "<span data-bind=\"text: formatNumber(NoTruoc,2)\"></span>");
            content1 = content1.Replace("{NoSau}", "<span data-bind=\"text: formatNumber(NoSau,2)\"></span>");
            content1 = content1.Replace("{TienThuaTraKhach}", "<span data-bind=\"text: formatNumber(TienThua,2)\"></span>");
            content1 = content1.Replace("{TienKhachThieu}", "<span data-bind=\"text: formatNumber(TienKhachThieu,2)\"></span>");
            content1 = content1.Replace("{DiemGiaoDich}", "<span data-bind=\"text: DiemGiaoDich\"></span>");
            content1 = content1.Replace("{TongTienThue}", "<span data-bind=\"text: formatNumber(TongTienThue,2)\"></span>");
            content1 = content1.Replace("{TongThueKhachHang}", "<span data-bind=\"text: formatNumber(TongThueKhachHang,2)\"></span>");
            content1 = content1.Replace("{TongGiamGiaHang}", "<span data-bind=\"text: formatNumber(TongGiamGiaHang,2)\"></span>");
            content1 = content1.Replace("{TongTienHangChuaChietKhau}", "<span data-bind=\"text: formatNumber(TongTienHangChuaCK,2)\"></span>");
            content1 = content1.Replace("{PTChietKhauHD}", "<span data-bind=\"text: TongChietKhau\"></span>");
            content1 = content1.Replace("{TienBangChu}", "<span data-bind=\"text: TienBangChu\"></span>");
            content1 = content1.Replace("{KH_TienBangChu}", "<span data-bind=\"text: KH_TienBangChu\"></span>");

            content1 = content1.Replace("{TienPOS}", "<span data-bind=\"text: formatNumber(TienATM)\"></span>");
            content1 = content1.Replace("{TienMat}", "<span data-bind=\"text: formatNumber(TienMat)\"></span>");
            content1 = content1.Replace("{TienChuyenKhoan}", "<span data-bind=\"text: formatNumber(ChuyenKhoan)\"></span>");
            content1 = content1.Replace("{TraLaiTienDatCoc}", "<span data-bind=\"text: formatNumber(TraLaiTienDatCoc)\"></span>");
            content1 = content1.Replace("{TTBangTienCoc}", "<span data-bind=\"text: formatNumber(TTBangTienCoc)\"></span>");
            content1 = content1.Replace("{TienDoiDiem}", "<span data-bind=\"text: formatNumber(TienDoiDiem)\"></span>");
            content1 = content1.Replace("{TienTheGiaTri}", "<span data-bind=\"text: formatNumber(TienTheGiaTri)\"></span>");

            content1 = content1.Replace("{TongGiamGiaHD_HH}", "<span data-bind=\"text: formatNumber(TongGiamGiaHD_HH)\"></span>");
            content1 = content1.Replace("{ChietKhauNVHoaDon}", "<span data-bind=\"text: ChietKhauNVHoaDon\"></span>");
            content1 = content1.Replace("{ChietKhauNVHoaDon_InGtriCK}", "<span data-bind=\"text: ChietKhauNVHoaDon_InGtriCK\"></span>");

            content1 = content1.Replace("{TienThuaTraKhach}", "<span data-bind=\"text: formatNumber(TienThua)\"></span>");
            content1 = content1.Replace("{TienKhachThieu}", "<span data-bind=\"text: formatNumber(TienKhachThieu)\"></span>");
            content1 = content1.Replace("{BH_TienThua}", "<span data-bind=\"text: BH_TienThua\"></span>");
            content1 = content1.Replace("{BH_ConThieu}", "<span data-bind=\"text: BH_ConThieu\"></span>");
            content1 = content1.Replace("{HD_TienThua}", "<span data-bind=\"text: HD_TienThua\"></span>");
            content1 = content1.Replace("{HD_ConThieu}", "<span data-bind=\"text: HD_ConThieu\"></span>");
            content1 = content1.Replace("{BH_TienBangChu}", "<span data-bind=\"text: BH_TienBangChu\"></span>");

            content1 = content1.Replace("{BaoHiemDaTra}", "<span data-bind=\"text: formatNumber(BaoHiemDaTra)\"></span>");
            content1 = content1.Replace("{TenBaoHiem}", "<span data-bind=\"text: TenBaoHiem\"></span>");
            content1 = content1.Replace("{BH_SDT}", "<span data-bind=\"text: BH_SDT\"></span>");
            content1 = content1.Replace("{BH_Email}", "<span data-bind=\"text: BH_Email\"></span>");
            content1 = content1.Replace("{BH_DiaChi}", "<span data-bind=\"text: BH_DiaChi\"></span>");
            content1 = content1.Replace("{BH_TenLienHe}", "<span data-bind=\"text: BH_TenLienHe\"></span>");
            content1 = content1.Replace("{BH_SDTLienHe}", "<span data-bind=\"text: BH_SDTLienHe\"></span>");

            content1 = content1.Replace("{SoVuBaoHiem}", "<span data-bind=\"text: SoVuBaoHiem\"></span>");
            content1 = content1.Replace("{KhauTruTheoVu}", "<span data-bind=\"text: KhauTruTheoVu\"></span>");
            content1 = content1.Replace("{PTGiamTruBoiThuong}", "<span data-bind=\"text: PTGiamTruBoiThuong\"></span>");
            content1 = content1.Replace("{GiamTruBoiThuong}", "<span data-bind=\"text: GiamTruBoiThuong\"></span>");
            content1 = content1.Replace("{TongTienThueBaoHiem}", "<span data-bind=\"text: TongTienThueBaoHiem\"></span>");
            content1 = content1.Replace("{PTThueBaoHiem}", "<span data-bind=\"text: PTThueBaoHiem\"></span>");
            content1 = content1.Replace("{PTThueHoaDon}", "<span data-bind=\"text: PTThueHoaDon\"></span>");
            content1 = content1.Replace("{BHThanhToanTruocThue}", "<span data-bind=\"text: BHThanhToanTruocThue\"></span>");
            content1 = content1.Replace("{TongTienBHDuyet}", "<span data-bind=\"text: TongTienBHDuyet\"></span>");

            content1 = content1.Replace("{ChuXe}", "<span data-bind=\"text: ChuXe\"></span>");
            content1 = content1.Replace("{ChuXe_SDT}", "<span data-bind=\"text: ChuXe_SDT\"></span>");
            content1 = content1.Replace("{ChuXe_DiaChi}", "<span data-bind=\"text: ChuXe_DiaChi\"></span>");
            content1 = content1.Replace("{ChuXe_Email}", "<span data-bind=\"text: ChuXe_Email\"></span>");

            content1 = content1.Replace("{CoVan_SDT}", "<span data-bind=\"text: CoVan_SDT\"></span>");
            content1 = content1.Replace("{PTN_GhiChu}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: PTN_GhiChu\"></span>");

            content1 = content1 + " <p data-bind=\"css: {pageBreak: $root.Count_ListHoaDons() > 1 && $index() != ($root.Count_ListHoaDons()- 1) }\"></p>  </div>  ";

            return content1;
        }

        /// <summary>
        /// Hàm replace Các tham số để bind dữ liệu khi in (dùng cho thẻ giá trị)
        /// </summary>
        /// <param name="content1"></param>
        /// <returns></returns>

        private string ReaplaceMauIn_TheGiaTri(string content1)
        {
            content1 = Replace_ThongTinChung(content1);
            content1 = content1.Replace("{MaKhachHang}", "<span data-bind=\"text: InforHDprintf().MaKhachHang\"></span>");
            content1 = content1.Replace("{TenKhachHang}", "<span data-bind=\"text: InforHDprintf().TenKhachHang\"></span>");
            content1 = content1.Replace("{DiaChi}", "<span data-bind=\"text: InforHDprintf().DiaChiKhachHang\"></span>");
            content1 = content1.Replace("{DienThoai}", "<span data-bind=\"text: InforHDprintf().DienThoaiKhachHang\"></span>");
            content1 = content1.Replace("{NhanVienBanHang}", "<span data-bind=\"text: InforHDprintf().NguoiTao\"></span>");
            content1 = content1.Replace("{TienBangChu}", "<span data-bind=\"text: InforHDprintf().TienBangChu\"></span>");

            content1 = content1.Replace("{MucNap}", "<span data-bind=\"text: InforHDprintf().MucNap\"></span>");
            content1 = content1.Replace("{KhuyenMai}", "<span data-bind=\"text: InforHDprintf().KhuyenMai\"></span>");
            content1 = content1.Replace("{ThanhTien}", "<span data-bind=\"text: InforHDprintf().TongTienHang\"></span>");
            content1 = content1.Replace("{TongTien}", "<span data-bind=\"text: InforHDprintf().MucNap\"></span>");
            content1 = content1.Replace("{ChietKhauHoaDon}", "<span data-bind=\"text: InforHDprintf().TongGiamGia\"></span>");
            content1 = content1.Replace("{TongCong}", "<span data-bind=\"text: InforHDprintf().PhaiThanhToan\"></span>");
            content1 = content1.Replace("{DaThanhToan}", "<span data-bind=\"text: InforHDprintf().DaThanhToan\"></span>");
            content1 = content1.Replace("{DienGiai}", "<span style=\"white-space:pre-wrap\" data-bind=\"text: InforHDprintf().DienGiai\"></span>");

            // the gia tri
            content1 = content1.Replace("{TongTaiKhoanThe}", "<span data-bind=\"text: InforHDprintf().TongTaiKhoanThe\"></span>");
            content1 = content1.Replace("{TongSuDungThe}", "<span data-bind=\"text: InforHDprintf().TongSuDungThe\"></span>");
            content1 = content1.Replace("{SoDuConLai}", "<span data-bind=\"text: InforHDprintf().SoDuConLai\"></span>");

            content1 = content1.Replace("{TienMat}", "<span data-bind=\"text: InforHDprintf().TienMat\"></span>");
            content1 = content1.Replace("{TienPOS}", "<span data-bind=\"text: InforHDprintf().TienATM\"></span>");
            content1 = content1.Replace("{TienChuyenKhoan}", "<span data-bind=\"text: formatNumber(InforHDprintf().ChuyenKhoan)\"></span>");
            content1 = content1.Replace("{NoSau}", "<span data-bind=\"text: formatNumber(InforHDprintf().NoSau)\"></span>");
            content1 = content1.Replace("{PhuongThucTT}", "<span data-bind=\"text: InforHDprintf().PhuongThucTT\"></span>");

            return content1;
        }

        private string ReaplaceMauIn_PhieuLuong(string content1)
        {
            content1 = string.Concat("<style> .pageBreak {page-break-before:always;}</style >",
                " <div data-bind=\"foreach: ListHoaDon_ChiTietHoaDonPrint\" >", content1);
            content1 = content1.Replace("{TenCuaHang}", "<span data-bind=\"text: TenCuaHang\"></span>");
            content1 = content1.Replace("{DienThoaiCuaHang}", "<span data-bind=\"text: DienThoaiCuaHang\"></span>");
            content1 = content1.Replace("{DiaChiCuaHang}", "<span data-bind=\"text: DiaChiCuaHang\"></span>");
            content1 = content1.Replace("{TenChiNhanh}", "<span data-bind=\"text: TenChiNhanh\"></span>");
            content1 = content1.Replace("{DienThoaiChiNhanh}", "<span data-bind=\"text: DienThoaiChiNhanh\"></span>");
            content1 = content1.Replace("{DiaChiChiNhanh}", "<span data-bind=\"text: DiaChiChiNhanh\"></span>");
            content1 = content1.Replace("{Logo}", "<img data-bind=\"attr: {src: LogoCuaHang}\" style=\"width:100%\" />");

            content1 = content1.Replace("{NgayLapPhieu}", "<span data-bind=\"text: NgayLapPhieu\"></span>");
            content1 = content1.Replace("{NgayTao}", "<span data-bind=\"text: NgayTao\"></span>");
            content1 = content1.Replace("{Ngay}", "<span data-bind=\"text: Ngay\"></span>");
            content1 = content1.Replace("{Thang}", "<span data-bind=\"text: Thang\"></span>");
            content1 = content1.Replace("{Nam}", "<span data-bind=\"text: Nam\"></span>");
            content1 = content1.Replace("{TenBangLuong}", "<span data-bind=\"text: TenBangLuong\"></span>");
            content1 = content1.Replace("{MaBangLuongChiTiet}", "<span data-bind=\"text: MaBangLuongChiTiet\"></span>");
            content1 = content1.Replace("{MaNhanVien}", "<span data-bind=\"text: MaNhanVien\"></span>");
            content1 = content1.Replace("{TenNhanVien}", "<span data-bind=\"text: TenNhanVien\"></span>");

            content1 = content1.Replace("{NgayCongChuan}", "<span data-bind=\"text: NgayCongChuan\"></span>");
            content1 = content1.Replace("{NgayCongThuc}", "<span data-bind=\"text: NgayCongThuc\"></span>");
            content1 = content1.Replace("{LuongCoBan}", "<span data-bind=\"text: LuongCoBan\"></span>");
            content1 = content1.Replace("{LuongChinh}", "<span data-bind=\"text: LuongChinh\"></span>");
            content1 = content1.Replace("{LuongOT}", "<span data-bind=\"text: LuongOT\"></span>");
            content1 = content1.Replace("{PhuCapCoBan}", "<span data-bind=\"text: PhuCapCoBan\"></span>");
            content1 = content1.Replace("{PhuCapKhac}", "<span data-bind=\"text: PhuCapKhac\"></span>");
            content1 = content1.Replace("{TongGiamTru}", "<span data-bind=\"text: TongGiamTru\"></span>");
            content1 = content1.Replace("{ChietKhau}", "<span data-bind=\"text: ChietKhau\"></span>");
            content1 = content1.Replace("{LuongSauGiamTru}", "<span data-bind=\"text: LuongSauGiamTru\"></span>");
            content1 = content1.Replace("{TruTamUngLuong}", "<span data-bind=\"text: TruTamUngLuong\"></span>");

            content1 = content1.Replace("{ThucLinh}", "<span data-bind=\"text: ThucLinh\"></span>");
            content1 = content1.Replace("{ThanhToan}", "<span data-bind=\"text: ThanhToan\"></span>");
            content1 = content1.Replace("{TienBangChu}", "<span data-bind=\"text: TienBangChu\"></span>");
            content1 = content1 + " <p data-bind=\"css: {pageBreak: $root.Count_ListHoaDons() > 1 && $index() != ($root.Count_ListHoaDons()- 1) }\"></p>  </div>  ";
            return content1;
        }

        /// <summary>
        /// Lấy danh sách mẫu in theo chi nhanh
        /// </summary>
        /// <param name="idChiNhanh"></param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<Object> GetAllMauIn_ByChiNhanh(Guid idChiNhanh)
        {
            try
            {
                var content1 = from mi in db.DM_MauIn
                               join ct in db.DM_LoaiChungTu
                               on mi.ID_LoaiChungTu equals ct.ID
                               where mi.ID_DonVi == idChiNhanh
                               select new
                               {
                                   ID = mi.ID,
                                   DuLieuMauIn = mi.DuLieuMauIn,
                                   MaLoaiChungTu = ct.MaLoaiChungTu,
                                   LaMacDinh = mi.LaMacDinh,
                                   TenMauIn = mi.TenMauIn,
                               };
                return content1;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ThietLapAPI_GetAllMauIn_ByChiNhanh: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get all mau in (all chi nhanh): Dùng chung mẫu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<Object> GetAllMauIn()
        {
            try
            {
                var content1 = from mi in db.DM_MauIn
                               join ct in db.DM_LoaiChungTu
                               on mi.ID_LoaiChungTu equals ct.ID
                               select new
                               {
                                   ID = mi.ID,
                                   DuLieuMauIn = mi.DuLieuMauIn,
                                   MaLoaiChungTu = ct.MaLoaiChungTu,
                                   LaMacDinh = mi.LaMacDinh,
                                   TenMauIn = mi.TenMauIn,
                               };
                return content1;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ThietLapAPI_GetAllMauIn: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        /// <summary>
        ///  Lấy file Default 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetFileMauIn(string file)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy classHTCongTy = new ClassHT_CongTy(db);
                var subDomain = CookieStore.GetCookieAes("SubDomain");
                string defaultFolder = HttpContext.Current.Server.MapPath("~/Template/MauIn");
                string folderCus = HttpContext.Current.Server.MapPath("~/Template/MauIn/");

                // combine 2 string --> full path
                string targetPath = Path.Combine(folderCus, file);
                string result = "";
                if (System.IO.File.Exists(targetPath))
                {
                    result = System.IO.File.ReadAllText(targetPath);
                }
                else
                {
                    targetPath = Path.Combine(defaultFolder, file);
                    result = System.IO.File.ReadAllText(targetPath);
                }

                // get img from HT_CongTy
                var data = classHTCongTy.Get(null);
                var logo = string.Empty;
                if (data != null)
                {
                    logo = data.DiaChiNganHang;
                }
                return result;
            }
        }

        [HttpGet]
        public IHttpActionResult CheckQuanLyLo()
        {

            var ht = db.HT_CauHinhPhanMem.FirstOrDefault().LoHang;
            return Json(ht ?? false);
        }



        //tin ngắn SMS
        //[System.Web.Http.AcceptVerbs("GET", "POST")]

        //public string SendSMSToSDT()
        //{
        //    string sdt = "0989861122";
        //    string noidung = "Cong Ty SSOFT VIET NAM kinh moi anh Pham Van Trinh dung 13h trua ngay 28-11-2018 den an tiec BBQ Trung Kinh HN";
        //    string brand = "YnaSpa";
        //    string test = banhang24.App_Start.App_API.SMSController.SendBrandnameJson(sdt, noidung, brand);
        //    return test;
        //}

        //kích hoạt tin SMS
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool CheckKichHoatSMS()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        HT_CongTy cty = db.HT_CongTy.FirstOrDefault();
                        cty.DangHoatDong = true;
                        db.Entry(cty).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool HuyKichHoatSMS()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        HT_CongTy cty = db.HT_CongTy.FirstOrDefault();
                        cty.DangHoatDong = false;
                        db.Entry(cty).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }



        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void PostBrandName([FromBody] JObject data)
        {
            RegisterServiceSm objBrandName = data["objBrandName"].ToObject<RegisterServiceSm>();
            string str = CookieStore.GetCookieAes("SubDomain");
            var SoDTCuaHang = new CuaHangDangKyService().Query.Where(p => p.SubDomain == str).FirstOrDefault().SoDienThoai;

            objBrandName.ID = Guid.NewGuid();
            objBrandName.ID_SupplierSms = new SupplierSmService().GetSupplierDefault().FirstOrDefault().ID;
            objBrandName.CreateDate = DateTime.Now;
            objBrandName.SoDienThoaiCuaHang = SoDTCuaHang;
            objBrandName.Status = 2;
            new RegisterServiceSmsService().InsertBrandName(objBrandName);

            var email = "dangky@open24.vn";
            CuaHangDangKy cuahang = Model_banhang24vn.DAL.CuaHangDangKyService.Get(str.ToLower().Trim());
            string subject = "Yêu cầu kích hoạt SMS";
            string body = "Open24 thân mến,</b><br /> Chúng tôi muốn cửa hàng của mình được kích hoạt tính năng SMS với BrandName là: " + objBrandName.Name + ". <br />Thanks,<br />" + cuahang.TenCuaHang;
            banhang24.App_Start.App_API.MailHelper.SendEmail(email, subject, body);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void PostMauTin([FromBody] JObject data)
        {
            HeThong_SMS_TinMau objTinMau = data["objMauTin"].ToObject<HeThong_SMS_TinMau>();
            objTinMau.ID = Guid.NewGuid();
            objTinMau.NgayTao = DateTime.Now;
            HeThong_SMS_TinMauService.InsertMauTin(objTinMau);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void PostTinNhan([FromBody] JObject data)
        {
            try
            {
                HeThong_SMS objTinNhan = data["objTinNhan"].ToObject<HeThong_SMS>();
                Guid idbrandName = data["ID_BrandName"].ToObject<Guid>();
                banhang24.App_Start.App_API.SMSController.SendBrandnameJson(objTinNhan, idbrandName);
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("PostTinNhan " + e.InnerException + e.Message);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void Post_ChuyenTienND([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HeThong_SMS_TaiKhoan objChuyenTien = data["objChuyenTien"].ToObject<HeThong_SMS_TaiKhoan>();
                objChuyenTien.ID = Guid.NewGuid();
                objChuyenTien.ThoiGian = DateTime.Now;
                db.HeThong_SMS_TaiKhoan.Add(objChuyenTien);
                db.SaveChanges();
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void Post_NaptienND([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Guid id_nguoidung = data["ID_NguoiNhanTien"].ToObject<Guid>();
                HT_NguoiDung admin = db.HT_NguoiDung.Where(p => p.LaAdmin == true).FirstOrDefault();
                CuaHangNapTienDichVu objNapTien = data["objNapTien"].ToObject<CuaHangNapTienDichVu>();
                string str = CookieStore.GetCookieAes("SubDomain");
                var SoDTCuaHang = new CuaHangDangKyService().Query.Where(p => p.SubDomain == str).FirstOrDefault().SoDienThoai;
                HT_NguoiDung nd = db.HT_NguoiDung.Where(p => p.ID == id_nguoidung).FirstOrDefault();
                Guid? idphieunhan = null;
                if (nd.LaAdmin == false)
                {
                    HeThong_SMS_TaiKhoan objnew = new HeThong_SMS_TaiKhoan();
                    objnew.ID = Guid.NewGuid();
                    objnew.ID_NguoiChuyenTien = admin.ID;
                    objnew.ID_NguoiNhanTien = nd.ID;
                    objnew.ThoiGian = DateTime.Now;
                    objnew.SoTien = (double)objNapTien.SoTien;
                    objnew.GhiChu = objNapTien.GhiChu;
                    objnew.TrangThai = objNapTien.TrangThai.Value;
                    db.HeThong_SMS_TaiKhoan.Add(objnew);
                    db.SaveChanges();

                    idphieunhan = objnew.ID;
                }
                objNapTien.ID = Guid.NewGuid();
                objNapTien.NgayTao = DateTime.Now;
                objNapTien.ID_PhieuNhan = idphieunhan;
                objNapTien.SoDienThoaiCuaHang = SoDTCuaHang;
                new CuaHangDangKyService().InsertDichvuNaptien(objNapTien);

                var email = "dangky@open24.vn";
                CuaHangDangKy cuahang = Model_banhang24vn.DAL.CuaHangDangKyService.Get(str.ToLower().Trim());
                string subject = "Nạp tiền tài khoản";
                string body = "Chị Hà xinh gái ơi,</b><br /> Em muốn nạp " + string.Format("{0:n0}", (double)objNapTien.SoTien).Replace(".", ",") + " ngàn VNĐ vào tài khoản: " + nd.TaiKhoan + "của gian hàng: " + cuahang.TenCuaHang + ". <br />Thanks,<br />";
                banhang24.App_Start.App_API.MailHelper.SendEmail(email, subject, body);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool CheckAdminLogin(Guid id_nd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HT_NguoiDung nguoidung = db.HT_NguoiDung.Where(p => p.ID == id_nd).FirstOrDefault();
                return nguoidung.LaAdmin;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void PutBrandName([FromBody] JObject data)
        {
            RegisterServiceSm objBrandName = data["objBrandName"].ToObject<RegisterServiceSm>();

            new RegisterServiceSmsService().UpdateBrandName(objBrandName);
        }

        public IHttpActionResult UpdateTrangThaiTaiKhoanSMS(string str, Guid id, int trangthai)
        {
            //trangthai: 1: đã thanh toán, 0: Xoa, 2: Chờ thanh toán 
            using (SsoftvnContext db = SystemDBContext.GetDBContext(str))
            {
                try
                {
                    HeThong_SMS_TaiKhoan htsmstk = db.HeThong_SMS_TaiKhoan.Where(p => p.ID == id).FirstOrDefault();
                    htsmstk.TrangThai = trangthai;
                    db.SaveChanges();
                    return UpdateSuccess();
                }
                catch (Exception e)
                {
                    return Exeption(e);
                }

            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void PutMauTin([FromBody] JObject data)
        {
            HeThong_SMS_TinMau objMauTin = data["objMauTin"].ToObject<HeThong_SMS_TinMau>();

            HeThong_SMS_TinMauService.UpdateTinMau(objMauTin);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void DeleteBrandName(Guid id)
        {
            new RegisterServiceSmsService().UpdateBrandNameDelete(id);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void DeleteMauTin(Guid id)
        {
            HeThong_SMS_TinMauService.DeleteMauTin(id);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<RegisterServiceSmDTO> GetallBrandName()
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            var SoDTCuaHang = new CuaHangDangKyService().Query.Where(p => p.SubDomain == str).FirstOrDefault().SoDienThoai;
            return new RegisterServiceSmsService().GetAllBrandName(SoDTCuaHang).ToList();
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<HeThong_SMS_TinMauDTO> GetAllMauTin()
        {
            return HeThong_SMS_TinMauService.GetAllMauTin().ToList();
        }

        public IHttpActionResult GetListLoaiTinSMS()
        {
            return Json(commonEnumHellper.ListLoaiTinSMS.Select(o => new { ID = o.Key, Name = o.Value, IsSelected = false }));
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetAllTinGui(GridModelTinNhan model)
        {
            //List<HeThong_SMSDTO> lst = HeThong_SMS_TinMauService.GetAllTinGui(model.FromDate, model.ToDate).ToList();
            List<HeThong_SMSDTO> lst = HeThong_SMS_TinMauService.GetListSMSSend(model.FromDate, model.ToDate, model.Status, model.TypeSMS).ToList();
            var totalRecord = lst.Count();
            lst = lst.Skip(model.currentPage * model.pageSize).Take(model.pageSize).ToList();
            var pageCount = System.Math.Ceiling(totalRecord / (float)model.pageSize);
            return Json(new { data = lst, TotalRecord = totalRecord, PageCount = pageCount });
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public RegisterServiceSmDTO GetBrandNameById(Guid id)
        {
            RegisterServiceSm branname = new RegisterServiceSmsService().GetBrandNameById(id);
            RegisterServiceSmDTO dto = new RegisterServiceSmDTO();
            dto.ID = branname.ID;
            dto.BrandName = branname.Name;
            dto.GhiChu = branname.Note;
            dto.Status = branname.Status;
            return dto;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HeThong_SMS_TinMauDTO GetMauTinByID(Guid id)
        {
            HeThong_SMS_TinMau mautin = HeThong_SMS_TinMauService.GetMauTinByID(id);
            HeThong_SMS_TinMauDTO dto = new HeThong_SMS_TinMauDTO();
            dto.ID = mautin.ID;
            dto.NoiDungTin = mautin.NoiDung;
            dto.LoaiTin = mautin.LoaiTin;
            dto.LaMacDinh = mautin.LaMacDinh;
            return dto;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool GetThongTinKH()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy classHTCongTy = new ClassHT_CongTy(db);
                IQueryable<HT_CongTy> congty = classHTCongTy.Gets(null);
                return congty.FirstOrDefault().DangHoatDong;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool CheckBrandNameExist(string nameBrand)
        {
            return new RegisterServiceSmsService().CheckBrandNameExist(nameBrand);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool CheckBrandNameExistEdit(string nameBrand, Guid id)
        {
            return new RegisterServiceSmsService().CheckBrandNameExistEdit(nameBrand, id);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IEnumerable<Object> GetThongTinNguoiDungByIDNV(Guid idnv)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var tbl = from nv in db.NS_NhanVien
                              join nva in db.NS_NhanVien_Anh on nv.ID equals nva.ID_NhanVien into ps
                              from p in ps.DefaultIfEmpty()
                              where nv.ID == idnv
                              select new
                              {
                                  ID = nv.ID,
                                  TenNhanVien = nv.TenNhanVien,
                                  DiaChi = nv.DiaChiCoQuan,
                                  NgaySinh = nv.NgaySinh,
                                  DienThoai = nv.DienThoaiDiDong,
                                  Email = nv.Email,
                                  Avatar = p == null ? "/Content/images/images-user.png" : p.URLAnh
                              };
                    return tbl.ToList();
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IEnumerable<Object> GetListNguoiDungNotLogin(Guid id_nd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var tbl = from nd in db.HT_NguoiDung
                              where nd.ID != id_nd
                              select new
                              {
                                  ID = nd.ID,
                                  TaiKhoan = nd.TaiKhoan,
                              };
                    return tbl.ToList();
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public JsonResult<JsonResultNapTien> GetListNapTienByCuaHang(int currentpage, int pageSize)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string str = CookieStore.GetCookieAes("SubDomain");
                var SoDTCuaHang = new CuaHangDangKyService().Query.Where(p => p.SubDomain == str).FirstOrDefault().SoDienThoai;
                IQueryable<Object> lst = new CuaHangDangKyService().GetAllCuaHangNapTien.Where(o => o.SoDienThoaiCuaHang == SoDTCuaHang && o.TrangThai == 1);
                int totalRecords = lst.Count();
                JsonResultNapTien json = new JsonResultNapTien
                {
                    Rowcount = totalRecords,
                    pageCount = System.Math.Ceiling(totalRecords * 1.0 / pageSize),
                    lst = lst.Skip(currentpage * pageSize).Take(pageSize)
                };
                return Json(json);
            }
        }

        public JsonResult<JsonResultChuyenNhanTien> GetListChuyenNhanTien(int currentpage, int pageSize, Guid id_nd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    string str = CookieStore.GetCookieAes("SubDomain");
                    var SoDTCuaHang = new CuaHangDangKyService().Query.Where(p => p.SubDomain == str).FirstOrDefault().SoDienThoai;
                    decimal? TongTienNapAdmin = new CuaHangDangKyService().GetAllCuaHangNapTien.Where(o => o.SoDienThoaiCuaHang == SoDTCuaHang && o.TrangThai == 1) != null ? new CuaHangDangKyService().GetAllCuaHangNapTien.Where(o => o.SoDienThoaiCuaHang == SoDTCuaHang && o.TrangThai == 1).Sum(p => p.SoTien) : 0;

                    List<HeThong_SMS_TaiKhoanDTO> lstAllHDs = new List<HeThong_SMS_TaiKhoanDTO>();
                    var tbl = from taikhoan in db.HeThong_SMS_TaiKhoan
                              where taikhoan.ID_NguoiChuyenTien == id_nd || taikhoan.ID_NguoiNhanTien == id_nd
                              orderby taikhoan.ThoiGian descending
                              select new
                              {
                                  ID = taikhoan.ID,
                                  ID_NguoiChuyenTien = taikhoan.ID_NguoiChuyenTien,
                                  ID_NguoiNhanTien = taikhoan.ID_NguoiNhanTien,
                                  SoTien = taikhoan.SoTien,
                                  GhiChu = taikhoan.GhiChu,
                                  ThoiGian = taikhoan.ThoiGian
                              };

                    foreach (var item in tbl)
                    {
                        HeThong_SMS_TaiKhoanDTO ht = new HeThong_SMS_TaiKhoanDTO();
                        ht.ID = item.ID;
                        ht.NguoiChuyenTien = db.HT_NguoiDung.Where(p => p.ID == item.ID_NguoiChuyenTien).FirstOrDefault().TaiKhoan;
                        ht.NguoiNhanTien = db.HT_NguoiDung.Where(p => p.ID == item.ID_NguoiNhanTien).FirstOrDefault().TaiKhoan;
                        ht.ID_NguoiChuyenTien = item.ID_NguoiChuyenTien;
                        ht.ID_NguoiNhanTien = item.ID_NguoiNhanTien;
                        ht.SoTien = item.SoTien;
                        ht.GhiChu = item.GhiChu;
                        ht.ThoiGian = item.ThoiGian;
                        lstAllHDs.Add(ht);
                    }

                    HT_NguoiDung nd = db.HT_NguoiDung.Where(p => p.ID == id_nd).FirstOrDefault();

                    double? tongtienchuyen = lstAllHDs.Where(p => p.ID_NguoiChuyenTien == id_nd).Sum(p => p.SoTien);
                    double? tongtiennhan = lstAllHDs.Where(p => p.ID_NguoiNhanTien == id_nd).Sum(p => p.SoTien);
                    double? tongtienguitien = db.HeThong_SMS.Where(p => p.ID_NguoiGui == id_nd && p.TrangThai == 100).FirstOrDefault() != null ? db.HeThong_SMS.Where(p => p.ID_NguoiGui == id_nd && p.TrangThai == 100).Sum(p => p.GiaTien * p.SoTinGui) : 0;
                    double? tienconlai = nd.LaAdmin == true ? (double?)TongTienNapAdmin + tongtiennhan - tongtienchuyen - tongtienguitien : tongtiennhan - tongtienchuyen - tongtienguitien;

                    int totalRecords = lstAllHDs.Count();
                    lstAllHDs = lstAllHDs.Skip(currentpage * pageSize).Take(pageSize).ToList();
                    JsonResultChuyenNhanTien json = new JsonResultChuyenNhanTien
                    {
                        TongTienNap = (double?)TongTienNapAdmin,
                        TongTienChuyen = tongtienchuyen,
                        TongTienDungGuiTien = tongtienguitien,
                        TongTienNhan = tongtiennhan,
                        TongTienConLai = tienconlai,
                        lstChuyenNhan = lstAllHDs.ToList(),
                        Rowcount = totalRecords,
                        pageCount = System.Math.Ceiling(totalRecords * 1.0 / pageSize),
                    };
                    return Json(json);
                }
            }
        }

        public double? GetSoDuCuaTaiKhoan(Guid idnd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string str = CookieStore.GetCookieAes("SubDomain");
                var SoDTCuaHang = new CuaHangDangKyService().Query.Where(p => p.SubDomain == str).FirstOrDefault().SoDienThoai;
                decimal? TongTienNapAdmin = new CuaHangDangKyService().GetAllCuaHangNapTien.Where(o => o.SoDienThoaiCuaHang == SoDTCuaHang && o.TrangThai == 1) != null ? new CuaHangDangKyService().GetAllCuaHangNapTien.Where(o => o.SoDienThoaiCuaHang == SoDTCuaHang && o.TrangThai == 1).Sum(p => p.SoTien) : 0;
                double tongtienchuyen = db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiChuyenTien == idnd).FirstOrDefault() != null ? db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiChuyenTien == idnd).Sum(p => p.SoTien) : 0;
                double tongtiennhan = db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiNhanTien == idnd).FirstOrDefault() != null ? db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiNhanTien == idnd).Sum(p => p.SoTien) : 0;
                double tongtienguitien = db.HeThong_SMS.Where(p => p.ID_NguoiGui == idnd && p.TrangThai == 100).FirstOrDefault() != null ? db.HeThong_SMS.Where(p => p.ID_NguoiGui == idnd && p.TrangThai == 100).Sum(p => p.GiaTien * p.SoTinGui) : 0;

                HT_NguoiDung nd = db.HT_NguoiDung.Where(p => p.ID == idnd).FirstOrDefault();

                return nd.LaAdmin == true ? (double?)TongTienNapAdmin + tongtiennhan - tongtienchuyen - tongtienguitien : tongtiennhan - tongtienchuyen - tongtienguitien;
            }
        }

        public double? GetGiaTienTrenTinNhan(Guid id_brand)
        {
            RegisterServiceSm resg = new RegisterServiceSmsService().GetBrandNameById(id_brand);
            SupplierSm supli = new RegisterServiceSmsService().GetSupplierByID(resg.ID_SupplierSms.Value);
            return (double?)supli.Price;
        }


    }

    public class GridModelTinNhan
    {
        public int pageSize { get; set; }
        public int currentPage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Status { get; set; }
        public int? TypeSMS { get; set; }
    }


}
public class FIleUpdate
{
    public string NameFile { get; set; }
    public string Value { get; set; }
}