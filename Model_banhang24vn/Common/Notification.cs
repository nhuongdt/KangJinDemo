using Model_banhang24vn.CustomView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model_banhang24vn.Common
{
    public static class Notification
    {
        public enum ErrorCode
        {
            success = 0,
            notfound,
            exception,
            exist,
            error,
            notactive

        }
        public enum StatusOrder
        {
            TaoMoi = 0,
            DangVanChuyen,
            HoanThanh,
            TraLai,
            dathanhtoan,
            thanhtoanvadangvanchuyen

        }
        public enum VersionStore
        {
            dungthu = 0,
            dakyhopdong,
            chuadangky

        }
        public enum StatusContract
        {
            TaoMoi = 0,
            DangXacThuc,
            DaXacThuc,
            Huy,
        }
        public enum payment
        {
            Banking = 0,
            Live,
            Cod
        }
        public enum Time
        {
            thang = 0,
            quy,
            nam,
            tatca,
            tuychinh,
            gio,
            ngay

        }
        public enum TypeContact
        {
            lienhe,
            datmua,
            dungthu

        }
        public enum TypeSalesSoftWare
        {
            tietkiem = 0,
            tieuchuan,
            chuyennghiep

        }
        public enum TypeSoftWare
        {
            Open24 = 0,
            Ssoft,

        }
        public enum StatusContact
        {
            Moi,
            Daxuly,
            huy
        }
        public enum NotificationSoftware
        {
            thongbaochung = 0,
            thongbaorieng,

        }
        public enum selectTime
        {
            homqua = 0,
            homnay,
            tuantruoc,
            tuannay,
            thangnay,
            thangtruoc,
            quynay,
            quytruoc,
            namnay,
            namtruoc,
            tuychon
        }

        public enum Device
        {
            tv = 0,
            tablet,
            mobile,
            desktop,

        }
        public enum ServiceSms
        {
            xoa = 0,
            dakichhoat,
            chokichhoat,
            huy,

        }
        public enum DichVuNapTien
        {
            xoa = 0,
            dathanhtoan,
            chothanhtoan
        }
        public static Dictionary<int, string> listDichVuNapTien = new Dictionary<int, string>
                                                    {
                                                        { (int)DichVuNapTien.chothanhtoan,"Chưa thanh toán"},
                                                       {(int)DichVuNapTien.dathanhtoan,"Đã thanh toán"},

                                                    };
        public enum HoTroNhom
        {
            theonhom = 0,
            theotinhnang

        }
        public static Dictionary<int, string> listTypeSalesSoftWare = new Dictionary<int, string>
                                                    {
                                                        { (int)TypeSalesSoftWare.tietkiem,"Gói tiết kiệm"},
                                                       {(int)TypeSalesSoftWare.tieuchuan,"Gói tiêu chuẩn"},
                                                         {(int)TypeSalesSoftWare.chuyennghiep,"Gói chuyên nghiệp"},

                                                    };
        public static Dictionary<int, string> listSoftWare = new Dictionary<int, string>
                                                    {
                                                        { (int)TypeSoftWare.Open24,"Open24"},
                                                       {(int)TypeSoftWare.Ssoft,"SsoftVN"}

                                                    };
        public static Dictionary<int, string> listStatusContact = new Dictionary<int, string>
                                                    {
                                                        { (int)StatusContact.Moi,"Mới"},
                                                       {(int)StatusContact.huy,"Hủy"},
                                                         {(int)StatusContact.Daxuly,"Đã xử lý"},

                                                    };
        public static Dictionary<int, string> TrangThaiOrder = new Dictionary<int, string>
                                                    {
                                                       {(int)StatusOrder.TaoMoi,"Tạo mới"},
                                                         {(int)StatusOrder.dathanhtoan,"Đã thanh toán"},
                                                            {(int)StatusOrder.DangVanChuyen,"Đang vận chuyển"},
                                                              { (int)StatusOrder.thanhtoanvadangvanchuyen,"Đang vận chuyên(đã thanh toán)"},
                                                                {(int)StatusOrder.HoanThanh,"Hoàn thành"},
                                                                  {(int)StatusOrder.TraLai,"Trả lại"},

                                                    };
        public static Dictionary<int, string> ListNotificationSoftware = new Dictionary<int, string>
                                                    {
                                                       {(int)NotificationSoftware.thongbaochung,"Thông báo chung"},
                                                         {(int)NotificationSoftware.thongbaorieng,"Thông báo riêng"},

                                                    };
        public static Dictionary<int, string> HinhThucVanChuyen = new Dictionary<int, string>
                                                    {
                                                       {(int)payment.Banking,"	Thanh toán bằng Internet Banking"},
                                                         {(int)payment.Cod,"COD - Thanh toán khi giao hàng"},
                                                         {(int)payment.Live,"Thanh toán trực tiếp tại Văn phòng OPen24"},

                                                    };
        public static Dictionary<int, string> TrangThaiContract = new Dictionary<int, string>
                                                    {
                                                       {(int)StatusContract.TaoMoi,"Tạo mới"},
                                                         {(int)StatusContract.DangXacThuc,"Đang xác thực"},
                                                         {(int)StatusContract.DaXacThuc,"Đã xác thực"},
                                                             {(int)StatusContract.Huy,"Hủy"},
                                                    };
        public static Dictionary<int, string> TrangThaiServiceSms = new Dictionary<int, string>
                                                    {
                                                       {(int)ServiceSms.xoa,"Hủy"},
                                                         {(int)ServiceSms.dakichhoat,"Đã kích hoạt"},
                                                         {(int)ServiceSms.chokichhoat,"Chờ kích hoạt"}
                                                    };
        public static Dictionary<int, string> Quy = new Dictionary<int, string>
                                                    {
                                                       {1,"I"},
                                                        {2,"II"},
                                                         {3,"III"},
                                                          {4,"IV"},
                                                    };
        public static Dictionary<int, string> ListVersionStore = new Dictionary<int, string>
        {
            {(int)VersionStore.dungthu,"Dùng thử"},
             {(int)VersionStore.dakyhopdong,"ký hợp đồng"},
        };
        public static bool IsDevice { get; set; }

        public static List<AppChat> ListAppChat { get { return (HttpContext.Current.Application["App_Chat_Online"] as List<AppChat>); } }

        public static Guid AppChatId { get { return HttpContext.Current.Session["AppChatId"] != null ? (Guid)HttpContext.Current.Session["AppChatId"] : new System.Guid(); } }

        public static string TagsTinTuc = "Open 24,tin tuc viet nam,tin tức việt nam,tin tức,tin tuc,tin tuc open,tin tức open";



        public static List<int> Nam { get { return Enumerable.Range(1945, DateTime.Now.Year - 1944).ToList(); } }

        public static List<int> Thang { get { return Enumerable.Range(1, 12).ToList(); } }
        public enum expiryDate
        {
            tatca = 0,
            conhan,
            hethan
        }
        public static Dictionary<int, string> HanSuDung = new Dictionary<int, string>
        {
            {(int)expiryDate.tatca,"--Hạn sử dụng--" },
              {(int)expiryDate.conhan,"Còn hạn" },
                {(int)expiryDate.hethan,"Hết hạn" },

        };
        public static Dictionary<string, string> KeyInTitle = new Dictionary<string, string>
        {
            {MaNganhNgheKinhDoanh.DoChoiTreEm,"Đồ chơi trẻ em" },
              {MaNganhNgheKinhDoanh.GiaDungDienMay,"Điện máy gia dụng" },
            {MaNganhNgheKinhDoanh.MyPham,"Mỹ phẩm" },
                {MaNganhNgheKinhDoanh.NhaHangCafe,"Nhà hàng, quán cafe" },
                    {MaNganhNgheKinhDoanh.NhaThuoc,"Nhà thuốc" },
                        {MaNganhNgheKinhDoanh.NoiThat,"Nội thất, xây dựng" },
                            {MaNganhNgheKinhDoanh.NongSanTP,"Nông sản thực phẩm" },
                                    {MaNganhNgheKinhDoanh.OtoXeMay,"Ô tô, xe máy, xe đạp điện" },
                                    {MaNganhNgheKinhDoanh.NhanSu,"Nhân sự" },
                             {MaNganhNgheKinhDoanh.SalonGym,"Salon tóc, phòng tập gym" },
                            {MaNganhNgheKinhDoanh.SieuThi,"Siêu thị" },
                            {MaNganhNgheKinhDoanh.SpaTMV,"Spa, thẩm mỹ viện" },
                            {MaNganhNgheKinhDoanh.ThietBiCongNghe,"Thiết bị công nghệ" },
                            {MaNganhNgheKinhDoanh.ThoiTrangPK,"Thời trang phụ kiện" },
                            {MaNganhNgheKinhDoanh.VanPhongPham,"Văn phòng phẩm" },
                                {MaNganhNgheKinhDoanh.Other,"Lĩnh vực khác" },
        };
        public static Dictionary<string, string> ListKeyInTitle = new Dictionary<string, string>
        {
            {MaNganhNgheKinhDoanh.DoChoiTreEm,"Đồ chơi trẻ em" },
            {MaNganhNgheKinhDoanh.MyPham,"Mỹ phẩm" },
             {MaNganhNgheKinhDoanh.NhaHangCafe,"Nhà hàng, quán cafe" },
              {MaNganhNgheKinhDoanh.NhaThuoc,"Nhà thuốc" },
              {MaNganhNgheKinhDoanh.PhongKham,"Phòng khám" },
               {MaNganhNgheKinhDoanh.NoiThat,"Nội thất, xây dựng" },
                {MaNganhNgheKinhDoanh.NongSanTP,"Nông sản thực phẩm" },
                {MaNganhNgheKinhDoanh.OtoXeMay,"Ô tô, xe máy, xe đạp điện" },
                {MaNganhNgheKinhDoanh.NhanSu,"Nhân sự" },
                {MaNganhNgheKinhDoanh.PhuTung,"Phụ tùng ô tô,xe máy" },
                 {MaNganhNgheKinhDoanh.SalonGym,"Salon tóc, phòng tập gym" },
                            {MaNganhNgheKinhDoanh.SieuThi,"Siêu thị" },
                            {MaNganhNgheKinhDoanh.SpaTMV,"Spa, thẩm mỹ viện" },
                            {MaNganhNgheKinhDoanh.ThoiTrangPK,"Thời trang phụ kiện" },
                            {MaNganhNgheKinhDoanh.VanPhongPham,"Văn phòng phẩm" },
                                {MaNganhNgheKinhDoanh.Other,"Lĩnh vực khác" },

        };
        public const int PageDefault = 10;
        public const int PageClientDefault = 5;
        public const int CustomerClientDefault = 8;
        public const string Messager_InsertSuccess = "Thêm mới thành công";
        public const string Messager_DeleteSuccess = "Xóa thành công";
        public const string Messager_UpdateSuccess = "Cập nhật thành công";
        public const string Messager_Exception = "Đã xảy ra lỗi vui lòng thử lại sau.";
    }
    public static class StaticRole
    {
        public const string BAIVIET_INSERT = "BAIVIET_THEMMOI";
        public const string BAIVIET_UPDATE = "BAIVIET_SUA";
        public const string BAIVIET_DELETE = "BAIVIET_XOA";
        public const string BAIVIET_VIEW = "BAIVIET_XEM";

        public const string USER_INSERT = "USER_INSERT";
        public const string USER_UPDATE = "USER_UPDATE";
        public const string USER_DELETE = "USER_DELETE";
        public const string USER_VIEW = "USER_VIEW";

        public const string USERGROUP_INSERT = "USERGROUP_INSERT";
        public const string USERGROUP_UPDATE = "USERGROUP_UPDATE";
        public const string USERGROUP_DELETE = "USERGROUP_DELETE";
        public const string USERGROUP_VIEW = "USERGROUP_VIEW";

        public const string NHOMBAIVIET_INSERT = "NHOMBAIVIET_THEMMOI";
        public const string NHOMBAIVIET_DELETE = "NHOMBAIVIET_XOA";
        public const string NHOMBAIVIET_UPDATE = "NHOMBAIVIET_SUA";
        public const string NHOMBAIVIET_VIEW = "NHOMBAIVIET_XEM";

        public const string KHACHHANG_VIEW = "KHACHHANG_XEM";
        public const string KHACHHANG_INSERT = "KHACHHANG_THEMMOI";
        public const string KHACHHANG_UPDATE = "KHACHHANG_SUA";
        public const string KHACHHANG_DELETE = "KHACHHANG_XOA";

        public const string CUAHANG_VIEW = "CUAHANG_XEM";
        public const string CUAHANG_UPDATE = "CUAHANG_SUA";
        public const string CUAHANG_SUAHSD = "CUAHANG_SUAHSD";
        public const string CUAHANG_XEMMA = "CUAHANG_XEMMA";

        public const string BUSINESS_VIEW = "BUSINESS_VIEW";
        public const string BUSINESS_DELETE = "BUSINESS_DELETE";
        public const string BUSINESS_INSERT = "BUSINESS_INSERT";
        public const string BUSINESS_UPDATE = "BUSINESS_UPDATE";

        public const string SALESDEVICES_VIEW = "SALESDEVICES_VIEW";
        public const string SALESDEVICES_UPDATE = "SALESDEVICES_UPDATE";
        public const string SALESDEVICES_INSERT = "SALESDEVICES_INSERT";
        public const string SALESDEVICES_DELETE = "SALESDEVICES_DELETE";

        public const string ORDER_VIEW = "ORDER_VIEW";
        public const string ORDER_UPDATE = "ORDER_UPDATE";
        public const string ORDER_INSERT = "ORDER_INSERT";
        public const string ORDER_DELETE = "ORDER_DELETE";

        public const string INTRODUCECUSTOMER_VIEW = "INTRODUCECUSTOMER_VIEW";
        public const string INTRODUCECUSTOMER_UPDATE = "INTRODUCECUSTOMER_UPDATE";

        public const string CUSTOMERCONTACT_VIEW = "CUSTOMERCONTACT_VIEW";

        public const string MENUTAG_VIEW = "MENUTAG_VIEW";
        public const string MENUTAG_UPDATE = "MENUTAG_UPDATE";

        public const string ADVERTISEMENT_UPDATE = "ADVERTISEMENT_UPDATE";
        public const string ADVERTISEMENT_VIEW = "ADVERTISEMENT_VIEW";

        public const string CUSTOMERCONTACTSALE_VIEW = "CUSTOMERCONTACTSALE_VIEW";

        public const string NOTIFICATIONSOFWARE_VIEW = "NOTIFICATIONSOFWARE_VIEW";

        public const string GOIDICHVU_VIEW = "GOIDICHVU_VIEW";
        public const string GOIDICHVU_INSERT = "GOIDICHVU_INSERT";
        public const string GOIDICHVU_UPDATE = "GOIDICHVU_UPDATE";
        public const string GOIDICHVU_DELETE = "GOIDICHVU_DELETE";

        public const string DICHVUSMS_VIEW = "DICHVUSMS_VIEW";
        public const string DICHVUSMS_UPDATE = "DICHVUSMS_UPDATE";

        public const string NAPTIEN_VIEW = "NAPTIEN_VIEW";
        public const string NAPTIEN_INSERT = "NAPTIEN_INSERT";
        public const string NAPTIEN_UPDATE = "NAPTIEN_UPDATE";
        public const string NAPTIEN_DELETE = "NAPTIEN_DELETE";

        public const string CAUHOI_VIEW = "CAUHOI_VIEW";
        public const string CAUHOI_INSERT = "CAUHOI_INSERT";
        public const string CAUHOI_UPDATE = "CAUHOI_UPDATE";
        public const string CAUHOI_DELETE = "CAUHOI_DELETE";

        public const string NHOMNGANH_VIEW = "NHOMNGANH_VIEW";
        public const string NHOMNGANH_INSERT = "NHOMNGANH_INSERT";
        public const string NHOMNGANH_UPDATE = "NHOMNGANH_UPDATE";
        public const string NHOMNGANH_DELETE = "NHOMNGANH_DELETE";

        public const string HT_TINHNANG_VIEW = "HT_TINHNANG_VIEW";
        public const string HT_TINHNANG_INSERT = "HT_TINHNANG_INSERT";
        public const string HT_TINHNANG_UPDATE = "HT_TINHNANG_UPDATE";
        public const string HT_TINHNANG_DELETE = "HT_TINHNANG_DELETE";

        public const string SEOURL_INSERT = "SEOURL_INSERT";
    }

    public static class CacheKey
    {
        public const string Customer_Home_Slider = "GetShowSlider";
        public const string Customer_All = "GetClient";
        public const string Customer_NewDate = "GetCustomerNewDate";
        public const string News_NewDate = "GetNewDate";
        public const string News_ViewCount = "GetNewsView";
        public const string News_Home_Slider = "GetAllArticleNewsHome";
        public const string Home_Dangky = "GetDataDangky";
    }
    public static class KeyGoiDichVu
    {
        public const string TietKiem = "GDV_TK";
        public const string TieuChuan = "GDV_TC";
        public const string ChuyenNghiep = "GDV_CN";
    }
    public class MaNganhNgheKinhDoanh
    {
        public const string SpaTMV = "TMV";
        public const string GiaDungDienMay = "DTDM";
        public const string VanPhongPham = "VPPQ";
        public const string SalonGym = "STPG";
        public const string MyPham = "MP";
        public const string ThoiTrangPK = "TTPK";
        public const string OtoXeMay = "OTXMXDD";
        public const string NhanSu = "NS";
        public const string PhuTung = "PTOTXM";
        public const string NhaThuoc = "NTYT";
        public const string PhongKham = "PKYT";
        public const string DoChoiTreEm = "DCTE";
        public const string NoiThat = "NT";
        public const string NongSanTP = "NSTP";
        public const string NhaHangCafe = "NHC";
        public const string SieuThi = "STMN";
        public const string ThietBiCongNghe = "TBCN";
        public const string Other = "Other";

        public const string TitleSpaTMV = "spa-tham-my-vien";
        public const string TitleGiaDungDienMay = "gia-dung-dien-may";
        public const string TitleVanPhongPham = "van-phong-pham";
        public const string TitleSalonGym = "salon-toc";
        public const string TitleMyPham = "my-pham";
        public const string TitleThoiTrangPK = "thoi-trang-phu-kien";
        public const string TitleOtoXeMay = "gara-oto-xe-may";
        public const string TitleNhanSu = "nhan-su";
        public const string TitlePhuTung= "phu-tung-oto-xe-may";
        public const string TitleNhaThuoc = "nha-thuoc";
        public const string TitlePhongKham = "phong-kham";
        public const string TitleDoChoiTreEm = "do-choi-tre-em";
        public const string TitleNoiThat = "noi-that";
        public const string TitleNongSanTP = "nong-san-thuc-pham";
        public const string TitleNhaHangCafe = "nha-hang-cafe";
        public const string TitleSieuThi = "sieu-thi-mini";
        public const string TitleThietBiCongNghe = "thiet-bi-cong-nghe";
        public const string TitleOther = "phan-mem-quan-ly-open24";

        public const string TitleLandingPage = "LandingPage";
    }
}
