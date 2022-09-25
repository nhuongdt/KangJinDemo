using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace banhang24
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Clear();
            bundles.ResetAll();
            BundleTable.EnableOptimizations = false;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"
                     ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                    "~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.unobtrusive*",
                    "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"));
            bundles.Add(new LessBundle("~/Content/less").Include("~/Content/bootstrap-datetimepicker-build.less"));
            bundles.Add(new StyleBundle("~/Content/GaraCss").Include(
                    "~/Content/Gara.css"
                    ));
            bundles.Add(new StyleBundle("~/Content/LoginCss").Include(
                    "~/Content/VariablesStyle.css",
                    "~/Content/Framework/Bootstrap/base/bootstrap.min.css",
                    "~/Content/login.css"
                 ));
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                    "~/Content/themes/base/jquery.ui.core.css",
                    "~/Content/themes/base/jquery.ui.resizable.css",
                    "~/Content/themes/base/jquery.ui.selectable.css",
                    "~/Content/themes/base/jquery.ui.accordion.css",
                    "~/Content/themes/base/jquery.ui.autocomplete.css",
                    "~/Content/themes/base/jquery.ui.button.css",
                    "~/Content/themes/base/jquery.ui.dialog.css",
                    "~/Content/themes/base/jquery.ui.slider.css",
                    "~/Content/themes/base/jquery.ui.tabs.css",
                    "~/Content/themes/base/jquery.ui.datepicker.css",
                    "~/Content/themes/base/jquery.ui.progressbar.css",
                    "~/Content/themes/base/jquery.ui.theme.css"));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/HangHoaList").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                      "~/Content/Framework/Knockout/knockout-jqAutocomplete.min.js",
                       "~/Scripts/Components/Ko-component.js",
                       "~/Scripts/BanHang/jqAutoProduct.js",
                    "~/Scripts/DanhMuc/SanPham/HangHoaList.js"));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/GiaBan").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                     "~/Content/Framework/Knockout/knockout-jqAutocomplete.min.js",
                    "~/Scripts/DanhMuc/SanPham/ChuongTrinhGia.js"));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/KiemKho").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/DanhMuc/SanPham/KiemKho.js"));
            bundles.Add(new ScriptBundle("~/bundles/DanhMucSMS").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/DoiTac/jqAutoCustomer.js",
                    "~/Scripts/ChamSocKhachHang/HTSMS.js"));
            bundles.Add(new ScriptBundle("~/bundles/DoiTac").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Content/Framework/Knockout/knockout-jqAutocomplete.js",
                    "~/Scripts/itemsjs.js",
                    "~/Scripts/BanHang/jqAutoProduct.js",
                    "~/Scripts/DoiTac/jqAutoCustomer.js",
                    "~/Scripts/DoiTac/LienHe_modal.js",
                    "~/Scripts/ChamSocKhachHang/_CongViecLichHen.js",
                    "~/Scripts/DoiTac/TrangThaiKH_modal.js",
                    "~/Scripts/DoiTac/ThemMoiKhachHang_modal.js",
                    "~/Scripts/DoiTac/KhachHang1_2.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/HoaHongGioiThieu").Include(
                  "~/Scripts/Components/Input.js",
                  "~/Scripts/Components/page-list.js",
                  "~/Scripts/Components/filter-checkbox.js",
                  "~/Scripts/Components/filter-chinhanh.js",
                  "~/Scripts/Components/filter-datetime.js",
                  "~/Scripts/Components/NhanVien_KhachHang.js",
                  "~/Scripts/DoiTac/HoaHongGioiThieu.js"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDich").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/GiaoDich.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDich/NhapHang").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/NhapHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/NhapHangChiTiet").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                     "~/Content/Framework/Knockout/knockout-jqAutocomplete.js",
                       "~/Scripts/Components/Ko-component.js",
                         "~/Scripts/BanHang/jqAutoProduct.js",
                    "~/Scripts/DoiTac/_ThemMoiNCC.js",
                    "~/Scripts/BanHang/_ThemMoiHangHoa.js",
                    "~/Scripts/GiaoDich/NhapHangChiTiet.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDich/TraHangNhap").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/TraHangNhap.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/TraHangNhapChiTiet").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                        "~/Content/Framework/Knockout/knockout-jqAutocomplete.js",
                    "~/Scripts/BanHang/jqAutoProduct.js",
                      "~/Scripts/Components/Ko-component.js",
                    "~/Scripts/GiaoDich/TraHangNhapChiTiet.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDichBan_Dat_Tra").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/GiaoDichBan_Dat_Tra.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/GoiDichVu").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/GoiDichVu.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/TheGiaTri").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                             "~/Content/Framework/Knockout/knockout-jqAutocomplete.min.js",
                          "~/Scripts/Components/Ko-component.js",
                    "~/Scripts/GiaoDich/TheGiaTri.js"));
            bundles.Add(new ScriptBundle("~/bundles/NhaHang").Include(
                  "~/Content/Framework/Knockout/knockout-{version}.js",
                  "~/Scripts/vue.min.js",
                    "~/Scripts/itemsjs.js",
                    "~/Scripts/BanHang/FormModel_BankAccount.js",
                    "~/Scripts/BanHang/FormModel_NewInvoice.js",
                    "~/Scripts/BanHang/_ThemMoiKhachHangOffline.js",
                    "~/Scripts/BanHang/NhaHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/BanHang").Include(
                    "~/Scripts/BanHang/_ThemMoiHangHoa.js",
                    "~/Scripts/BanHang/_LichHen.js",
                    "~/Scripts/BanHang/BanHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/BanLe").Include(
                    "~/Scripts/BanHang/_ThemMoiHangHoa.js",
                    "~/Scripts/BanHang/_LichHen.js",
                    "~/Scripts/BanHang/BanLe.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/Gara").Include(
                    "~/Scripts/Components/Ko-component.js",
                    "~/Scripts/BanHang/_ThemMoiHangHoa.js",
                    "~/Scripts/BanHang/_LichHen.js",
                    "~/Scripts/Gara/Gara.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/Kitchen").Include(
                "~/Content/Framework/Knockout/knockout-{version}.js",
                   "~/Scripts/jquery.signalR-2.2.2.min.js",
                    "~/Scripts/BanHang/NhaBep.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/DisplayCustomer").Include(
                  "~/Scripts/BanHang/DisplayCustomer.js"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/BanHangBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoBanHangChiTiet.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/OptinFormKhachHang").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Marketing/OptinFormKhachHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/OptinFormKhachHangMaNhung").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/SDK/OptinFromKhachHang_MaNhung.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/DanhSachDangKyOpntinForm").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Marketing/DanhSachDangKyOpntinForm.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportGoiDichVu").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Report/BaoCaoGoiDichVu.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportBanHang").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/DoiTac/jqAutoCustomer.js",
                    "~/Scripts/Report/BaoCaoBanHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportDatHang").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Report/BaoCaoDatHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportNhapHang").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Report/BaoCaoNhapHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportKho").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Report/BaoCaoKho.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportTaiChinh").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Report/BaoCaoTaiChinh.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportChietKhau").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Report/BC_HoaHong.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ReportNhanVien").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Report/BaoCaoNhanVien.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/NS_CaLamViec").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/NhanSu/CaLamViec.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/NS_PhanCa").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/NhanSu/PhanCa.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/DieuChinh").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/knockout-jqAutocomplete.min.js",
                    "~/Scripts/GiaoDich/DieuChinh.js"
                    )); 
            bundles.Add(new ScriptBundle("~/bundles/DieuChinhChiTiet").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                       "~/Scripts/Components/Ko-component.js",
                    "~/Scripts/GiaoDich/PhieuDieuChinhChiTiet.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/HangHoaBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoHangHoa.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/NhanVienBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoNhanVien.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/NhaCungCapBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoNhaCungCap.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/KhachHangBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoKhachHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/DatHangBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoDatHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/TaiChinhBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoTaiChinh.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/CuoiNgayBC").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BaoCao/BaoCaoCuoiNgay.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/LichSuThaoTac").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/ThietLap/LichSuThaoTac.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ChotSo").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/ThietLap/ThietLapChotSo.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/TongQuan").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Home/TongQuan.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/TongQuanHRM").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Home/TongQuanHRM.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/XuatHuy").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/XuatHuy.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/PhieuDieuChinh").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/PhieuDieuChinh.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/CaiDatHoaHong").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                      "~/Content/Framework/Knockout/knockout-jqAutocomplete.min.js",
                    "~/Content/Treeview/gijgo.js",
                    "~/Scripts/Thietlap/ChietKhauNV.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/KhuyenMai").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                      "~/Content/Framework/Knockout/knockout-jqAutocomplete.min.js",
                    "~/Scripts/BanHang/jqAutoProduct.js",
                    "~/Scripts/ThietLap/QuanLyKhuyenMai.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/Spa").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BanHang/Spa2.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/NhanVienList").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Content/faloading/Exportt.js",
                      "~/Content/Treeview/gijgo.js",
                    "~/Scripts/Thietlap/NhanVienList.js"));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/ChietKhauList").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/Thietlap/ChietKhauList.js"));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/TuVanList").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/ChamSocKhachHang/TuVan.js"));
            bundles.Add(new ScriptBundle("~/bundles/WorkSchedule").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BanHang/jqAutoProduct.js",
                    "~/Scripts/DoiTac/jqAutoCustomer.js",
                    "~/Scripts/ChamSocKhachHang/_CongViecLichHen.js",
                    "~/Scripts/ChamSocKhachHang/_ModalSMS.js",
                    "~/Scripts/ChamSocKhachHang/CongViec2.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/LichHenList").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/DoiTac/jqAutoCustomer.js",
                    "~/Content/calendar/js/underscore-min.js",
                    "~/Content/calendar/js/calendar.js",
                    "~/Scripts/ChamSocKhachHang/LichHen.js"));
            bundles.Add(new ScriptBundle("~/bundles/AppDanhMuc/PhanHoiList").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/ChamSocKhachHang/PhanHoi.js"));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDich/ChuyenHang").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/ChuyenHang.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDich/ChuyenHang2").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/GiaoDich/ChuyenHang2.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDich/ChuyenHangChiTiet").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BanHang/jqAutoProduct.js",
                    "~/Content/js/Common.js",
                    "~/Scripts/GiaoDich/ChuyenHangChiTiet.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/GiaoDich/XuatKhoChiTiet").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Scripts/BanHang/jqAutoProduct.js",
                    "~/Content/js/Common.js",
                    "~/Scripts/GiaoDich/XuatKhoChiTiet.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/MauIn").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                   "~/Scripts/Thietlap/QLMauIn.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ThietLap").Include(
                   "~/Content/Framework/Knockout/knockout-{version}.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/SoQuy2").Include(
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                       "~/Content/Framework/Knockout/knockout-jqAutocomplete.min.js",
                    "~/Scripts/SoQuy/SoQuy_2.js"
                    ));
            bundles.Add(new StyleBundle("~/Content/CssFramework").Include(
                    "~/Content/Framework/Bootstrap/base/bootstrap.min.css",
                    "~/Content/Framework/Bootstrap/plugins/BootstrapDatePicker/bootstrap-datepicker.css",
                    "~/Content/Framework/Jquery/css/jquery.ui.timepicker.css",
                    "~/Content/Framework/Jquery/css/jquery-ui.css",
                    "~/Content/Framework/Jquery/css/jquery.bxslider.css",
                    "~/Content/Framework/Jquery/plugins/DateTimePicker/jquery.datetimepicker.css",
                    "~/Content/Framework/DateRangePicker/daterangepicker.css",
                    "~/Content/Framework/DateRangePicker/bootstrap-datepicker.css",
                    "~/Content/Framework/fullcalendarIo/css/fullcalendar.css",
                    "~/Content/Framework/PrintJS/print.min.css",
                    "~/Content/Framework/Gijgo/gijgo.css"
                    //"~/Content/Framework/fontawesome-pro-5.15.3-web/css/all.css",
                    //"~/Content/Framework/fontawesome-pro-5.15.3-web/css/v4-shims.css"
                    //"~/Content/all.css",
                    //"~/Content/v4-shims.css"
                    ).Include("~/Content/Framework/fontawesome-pro-5.15.3-web/css/all.css", new CssRewriteUrlTransform())
                    .Include("~/Content/Framework/fontawesome-pro-5.15.3-web/css/v4-shims.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/Open24Css").Include(
                    "~/Content/VariablesStyle.css",
                    "~/Content/Gara.css",
                    "~/Content/style.css", 
                    "~/Content/main.css" ,
                    "~/Content/css/op-component.css"
                    ));
            bundles.Add(new StyleBundle("~/Content/BanLeCss").Include(
               "~/Content/Banhang.css"
                   ));
            bundles.Add(new ScriptBundle("~/bundles/JsFramework").Include(
                    "~/Content/Framework/Jquery/jquery/jquery-{version}.js",
                    "~/Content/Framework/Bootstrap/base/bootstrap.js",
                    "~/Content/Framework/Vue.js/v2.6.14/vue.min.js",
                    //"~/Content/Framework/vueJs/vue.min.js",
                    "~/Content/Framework/Knockout/knockout-{version}.js",
                    "~/Content/Framework/Gijgo/gijgo.js",
                    "~/Content/Framework/Bootstrap/plugins/bootstrapNotify/bootstrap-notify.js",
                    "~/Content/Framework/Jquery/plugins/jquery.bootstrap-growl.min.js",
                    "~/Content/Framework/Jquery/plugins/jquery.bxslider.js",
                    "~/Content/Framework/Jquery/plugins/jquery.cookie.js",
                    "~/Content/Framework/Jquery/plugins/jquery.signalR-2.2.2.min.js",
                    "~/Content/Framework/Jquery/JqueryUI/jquery-ui.js",
                    "~/Content/Framework/Knockout/knockout-jqAutocomplete.js",
                    "~/Content/Framework/JsBarcode/JsBarcode.all.min.js",
                    "~/Content/Framework/Dexie/dexie.js",
                    "~/Content/Framework/RespondJS/respond.min.js",
                    "~/Content/Framework/PrintJS/print.min.js",
                    "~/Content/Framework/OpenJS/shortcut.js",
                    "~/Content/Framework/Moment/moment.js",
                    "~/Content/Framework/Moment/moment-with-locales.js",
                    "~/Content/Framework/Jquery/plugins/DateTimePicker/jquery.datetimepicker.full.min.js",
                    "~/Content/Framework/Jquery/plugins/DateTimePicker/jquery.maskedinput.min.js", 
                    "~/Content/Framework/Jquery/plugins/DateTimePicker/jquery.ui.datepicker-vi-VN.js",
                    "~/Content/Framework/DateRangePicker/daterangepicker.js",
                    "~/Content/Framework/fullcalendarIo/js/fullcalendar.js"
                      //"~/Scripts/fontawesome/all.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/JsStatic").Include(
                    "~/Content/js/main.js",
                    "~/Scripts/ssoftvn.js",
                    "~/Content/js/Common.js",
                    "~/Scripts/BanHang/Public.js",
                    "~/Content/faloading/progressBar.js",
                    "~/Content/js/op-responsive.js"
                    ));
            BundleTable.EnableOptimizations = false;
        }
    }
}