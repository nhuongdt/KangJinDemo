using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Reflection;
using Model_banhang24vn.DAL;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.Configuration;
using FastMember;

namespace Open24.Areas.AdminPage.Controllers
{
    [RBACAuthorize]
    public class StoreRegistrationController : BaseController
    {
        #region Declare 
        private CuaHangDangKyService _CuaHangDangKyService;

        public StoreRegistrationController()
        {
            _CuaHangDangKyService = new CuaHangDangKyService();
        }

        #endregion

        #region View
        // GET: AdminPage/StoreRegistration
        [RBACAuthorize(RoleKey = StaticRole.CUAHANG_VIEW)]
        public ActionResult Index()
        {
            var checkRoleView = new RoleStore()
            {
                RoleView = contant.CheckRole(StaticRole.CUAHANG_XEMMA),
                RoleInsert = false,
                RoleUpdate = contant.CheckRole(StaticRole.CUAHANG_UPDATE),
                RoleUpdateNews = contant.CheckRole(StaticRole.CUAHANG_SUAHSD),
                RoleDelete = false,
                ServceSms_View = contant.CheckRole(StaticRole.DICHVUSMS_VIEW),
                ServceSms_Update = contant.CheckRole(StaticRole.DICHVUSMS_UPDATE),
                Recharge_Insert = contant.CheckRole(StaticRole.NAPTIEN_INSERT),
                Recharge_Update = contant.CheckRole(StaticRole.NAPTIEN_UPDATE),
                Recharge_View = contant.CheckRole(StaticRole.NAPTIEN_VIEW),
                Recharge_Delete = contant.CheckRole(StaticRole.NAPTIEN_DELETE),
            };
            return View(checkRoleView);
        }
        #endregion View

        #region Event

        #endregion

        #region search end custom pageding grid

        /// <summary>
        /// Tìm kiếm gridview
        /// </summary>
        /// <param name="daTatable"></param>
        /// <returns></returns>
        public JsonResult GetDataForShearch(DataGridView daTatable)
        {
            var data = _CuaHangDangKyService.Search(daTatable.Search, daTatable.TypeHsd, daTatable.Status, daTatable.Version);
            if (daTatable.Sort == (int)GridPagedingHellper.GridSort.SortUp)
            {

                switch (daTatable.Columname)
                {
                    case (int)GridPagedingHellper.columtableStoreRegistration.bussines:
                        data = data.Where(o => o.NganhNgheKinhDoanh != null).OrderBy(o => o.NganhNgheKinhDoanh.TenNganhNghe);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.expiryDate:
                        data = data.OrderBy(o => o.HanSuDung);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.mobile:
                        data = data.OrderBy(o => o.SoDienThoai);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.name:
                        data = data.OrderBy(o => o.HoTen);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.status:
                        data = data.OrderBy(o => o.TrangThai);
                        break;
                    default:
                        data = data.OrderBy(o => o.NgayTao);
                        break;

                }
            }
            else
            {
                switch (daTatable.Columname)
                {
                    case (int)GridPagedingHellper.columtableStoreRegistration.bussines:
                        data = data.Where(o => o.NganhNgheKinhDoanh != null).OrderByDescending(o => o.NganhNgheKinhDoanh.TenNganhNghe);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.expiryDate:
                        data = data.OrderByDescending(o => o.HanSuDung);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.mobile:
                        data = data.OrderByDescending(o => o.SoDienThoai);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.name:
                        data = data.OrderByDescending(o => o.HoTen);
                        break;
                    case (int)GridPagedingHellper.columtableStoreRegistration.status:
                        data = data.OrderByDescending(o => o.TrangThai);
                        break;
                    default:
                        data = data.OrderByDescending(o => o.NgayTao);
                        break;

                }
            }
            daTatable.PageCount = (int)Math.Ceiling((double)data.Count() / daTatable.Limit);
            if (daTatable.PageCount == 0 || daTatable.PageCount == 1)
            {
                daTatable.PageCount = 1;
                daTatable.Page = 1;
                daTatable.Data = data.AsEnumerable().Select(o => new StoreRegistrationView
                {
                    Key = o.SubDomain,
                    Mobile = o.SoDienThoai,
                    SubDomain = o.SubDomain + ".Open24.vn",
                    Name = o.HoTen,
                    ExpiryDate = o.HanSuDung,
                    Business = o.NganhNgheKinhDoanh.TenNganhNghe,
                    Status = o.TrangThai,
                    SoLanKichHoat = o.SoLanKichHoat,
                    DiaChi = o.DiaChi,
                    Email = o.Email,
                    MaKichHoat = o.MaKichHoat,
                    NgayTao = o.NgayTao,
                    Quanhuyen = o.TinhThanh_QuanHuyen != null ? o.TinhThanh_QuanHuyen.QuanHuyen + " - " + o.TinhThanh_QuanHuyen.TinhThanh : string.Empty,
                    TenCuaHang = o.TenCuaHang,
                    DiaChiIP_DK = o.DiaChiIP_DK,
                    HeDieuHanh_DK = o.HeDieuHanh_DK,
                    KhuVuc_DK = o.KhuVuc_DK,
                    ThietBi_DK = o.ThietBi_DK,
                    ID_GoiDichVu = o.ID_GoiDichVu,
                    GoiDichVu = o.DM_GoiDichVu != null ? o.DM_GoiDichVu.TenGoi : string.Empty,
                    Version = o.version,
                    GhiChu = _CuaHangDangKyService.getNoteHistory(o.SubDomain),

                    VersionText = convertVerrsion(o.version)
                }).AsEnumerable();
            }
            else
            {
                daTatable.Data = data.Skip(daTatable.Limit * (daTatable.Page - 1)).Take(daTatable.Limit).AsEnumerable().Select(o => new StoreRegistrationView
                {
                    Key = o.SubDomain,
                    Mobile = o.SoDienThoai,
                    SubDomain = o.SubDomain + ".Open24.vn",
                    Name = o.HoTen,
                    ExpiryDate = o.HanSuDung,
                    Business = o.NganhNgheKinhDoanh.TenNganhNghe,
                    Status = o.TrangThai,
                    SoLanKichHoat = o.SoLanKichHoat,
                    DiaChi = o.DiaChi,
                    Email = o.Email,
                    MaKichHoat = o.MaKichHoat,
                    NgayTao = o.NgayTao,
                    Quanhuyen = o.TinhThanh_QuanHuyen != null ? o.TinhThanh_QuanHuyen.QuanHuyen + " - " + o.TinhThanh_QuanHuyen.TinhThanh : string.Empty,
                    TenCuaHang = o.TenCuaHang,
                    DiaChiIP_DK = o.DiaChiIP_DK,
                    HeDieuHanh_DK = o.HeDieuHanh_DK,
                    KhuVuc_DK = o.KhuVuc_DK,
                    ThietBi_DK = o.ThietBi_DK,
                    ID_GoiDichVu = o.ID_GoiDichVu,
                    GoiDichVu = o.DM_GoiDichVu != null ? o.DM_GoiDichVu.TenGoi : string.Empty,
                    Version = o.version,
                    GhiChu = _CuaHangDangKyService.getNoteHistory(o.SubDomain),

                    VersionText = convertVerrsion(o.version)
                }).AsEnumerable();
            }
            daTatable.PageItem = GridPagedingHellper.PageItems(daTatable.Page, daTatable.PageCount, data.Count());
            return Json(daTatable);

        }

        [HttpGet]
        public void ExportExcel(int? TypeHsd,int? Status,int? Version,string text)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("@hansudung", TypeHsd == null ? 0 : TypeHsd));
            sql.Add(new SqlParameter("@trangthai", Status == null ? -1 : Status));
            sql.Add(new SqlParameter("@version", Version == null ? -1 : Version));
            sql.Add(new SqlParameter("@date", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01)));
            var lst = new BanHang24vnContext().Database.SqlQuery<ExportCuaHangDangKy>("exec GetListCuaHangExport @hansudung," +
                 "@trangthai,@version,@date", sql.ToArray()).ToList();
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = StringExtensions.ConvertToUnSign(text.Trim());
                lst = lst.Where(o => StringExtensions.ConvertToUnSign(o.SoDienThoaiDangKy).Contains(text)
                                      || StringExtensions.ConvertToUnSign(o.HoTenKhachHang).Contains(text)
                                      || StringExtensions.ConvertToUnSign(o.TenGianHangDangKy).Contains(text)
                                      || StringExtensions.ConvertToUnSign(o.NganhNgheKinhDoanh).Contains(text)).ToList();
            }
            DataTable table = new DataTable();
            using (var reader = ObjectReader.Create(lst))
            {
                table.Load(reader);
            }
            table.TableName = "DanhSachCuaHangDangKy";
            table.Columns["SoDienThoaiDangKy"].SetOrdinal(0);
            table.Columns["TenCuaHang"].SetOrdinal(1);
            table.Columns["TenGianHangDangKy"].SetOrdinal(2);
            table.Columns["HoTenKhachHang"].SetOrdinal(3);
            table.Columns["DiaChi"].SetOrdinal(4);
            table.Columns["Email"].SetOrdinal(5);
            table.Columns["NganhNgheKinhDoanh"].SetOrdinal(6);
            table.Columns["GoiDichVu"].SetOrdinal(7);
            table.Columns["SoLanKichHoat"].SetOrdinal(8);
            table.Columns["TrangThai"].SetOrdinal(9);
            table.Columns["NgayDangKy"].SetOrdinal(10);
            table.Columns["HanSuDung"].SetOrdinal(11);
            table.Columns["PhienBan"].SetOrdinal(12);
            table.Columns["ThietBi_DK"].SetOrdinal(13);
            table.Columns["KhuVuc_DK"].SetOrdinal(14);
            table.Columns["HeDieuHanh_DK"].SetOrdinal(15);
            table.Columns["DiaChiIP_DK"].SetOrdinal(16);
            //table.Columns["GhiChu"].Caption="Ghi chú";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                wb.ShowRowColHeaders = true;
                wb.ShowZeros = true;
                wb.ShowGridLines = true;
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= DanhSachCuaHangDangKy.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        public string convertVerrsion(int? input)
        {
            if (input == (int)Notification.VersionStore.dungthu)
                return "Dùng thử";
            if (input == (int)Notification.VersionStore.dakyhopdong)
                return "Đã ký hợp đồng";

            return string.Empty;
        }


        #endregion
    }
}