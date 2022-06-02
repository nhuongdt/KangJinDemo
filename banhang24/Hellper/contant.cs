using libHT_NguoiDung;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.Hellper
{
    public class contant
    {
        public static bool checkUser()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Cookies["Account"] == null)
            {
                return false;
            }
            else
            {
                //var jsonconvert = httpRequest.Cookies["Account"].Value;
                //var json = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(jsonconvert), "SSOFTVN");
                //var ison2 = json.Replace("%0d%0a", "\r\n");
                //var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //var result = serializer.Deserialize<HT_NguoiDung>(ison2);
                //return result;
                return true;
            }
        }

        public static HT_NguoiDung GetUserCookies()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Cookies["Account"] == null)
            {
                return new HT_NguoiDung();
            }
            else
            {
                var jsonconvert = httpRequest.Cookies["Account"].Value;
                var json = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(jsonconvert), "SSOFTVN");
                var ison2 = json.Replace("%0d%0a", "\r\n");
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = serializer.Deserialize<HT_NguoiDung>(ison2);
                return result;
            }
        }
        public static bool CheckRolePermission(string Role)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                classHT_NguoiDung_Nhom classHTNguoiDungNhom = new classHT_NguoiDung_Nhom(db);

                var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                var cookie1 = CookieStore.GetCookieAes("Account");
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = serializer.Deserialize<HT_NguoiDung>(cookie1);
                if (result.LaAdmin == false)
                {
                    HT_NguoiDung_Nhom nguoidungnhom = classHTNguoiDungNhom.Gets(p => p.IDNguoiDung == ID_ND && p.ID_DonVi == result.ID_DonVi).FirstOrDefault();
                    //HT_NguoiDung_Nhom nguoidungnhom = classHT_NguoiDung_Nhom.Gets(p => p.IDNguoiDung == ID_ND).FirstOrDefault();
                    if (nguoidungnhom != null)
                    {
                        return classHTNguoiDung.Select_HT_Quyen_Nhom(nguoidungnhom.IDNhomNguoiDung)
                                                .Select(p => p.MaQuyen).Any(o => o.ToUpper().Equals(Role.ToUpper()));
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public static string IdNguoiDung { get { return CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID); } }
        public static string UserVerSion { get { return CookieStore.GetCookieAes(SystemConsts.UserVersion); } }
    }

    public class SystemConsts
    {
        public const string NGUOIDUNGID = "id_nguoidung";
        public const string UserVersion = "UserVersion";
    }
}