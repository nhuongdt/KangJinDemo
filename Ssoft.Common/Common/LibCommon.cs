using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ssoft.Common.Common
{
    public class LibCommon
    {

    }
    public class LibEnum
    {
        public enum TypeSoftWareSsoft
        {

            beauty = 1,
            gara,
            hrm
        }
        public enum Device
        {
            tv = 0,
            tablet,
            mobile,
            desktop,

        }
        public enum Menu
        {
            top=0,
            left,
            right,
            boottom
        }
        public enum IsStatus
        {
            an,
            hoatdong,
            xoa

        }
        public enum StatusGroupNews
        {
            tintuc=1,
            tuyendung
        }
        public enum StatusGroupNewsTag
        {
            tintuc=1,
            tuyendung,
            khachhang
        }
        public enum StatusContact
        {
            taomoi = 1,
            daxuly,
            hanthanh
        }
        public enum ErrorCode
        {
            Success = 1,
            Error,
        }
        public enum IsStatusTuyenDung
        {
            xoa=0,
            taomoi,
            dadoc,
            daphanhoi
        }
    }
    public static class LibNotification
    {
        public const int PageDefault = 10;
        public const int PageClientDefault = 5;
        public const int CustomerClientDefault = 8;
        public const string Messager_InsertSuccess = "Thêm mới thành công";
        public const string Messager_DeleteSuccess = "Xóa thành công";
        public const string Messager_UpdateSuccess = "Cập nhật thành công";
        public const string Messager_Exception = "Đã xảy ra lỗi vui lòng thử lại sau.";
        public const string LuckyGara = "Lucky Gara";
        public const string LuckyBeauty = "Lucky Beauty";
        public const string LuckyHrm = "Lucky Hrm";
        public const string KeyLuckyGara = "LK_GARA";
        public const string KeyLuckyBeauty = "LK_BEAUTY";
        public const string KeyLuckyHrm = "LK_HRM";
    }

    public class JsonViewModel<T>
    {
        public int ErrorCode { get; set; }
        public T Data { get; set; }
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
    }
}
