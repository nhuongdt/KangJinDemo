using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model.Web.API
{
    public class CookieStore
    {
        public static void SetCookie(string key, string value, TimeSpan expires, string Domain)
        {
            HttpCookie encodedCookie = HttpSecureCookie.Encode(new HttpCookie(key, value));
            if (HttpContext.Current.Response.Cookies[key] != null)
            {
                HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies[key].Value = encodedCookie.Value;
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }
        }

        public static void SetCookieAes(string key, string value, TimeSpan expires, string Domain)
        {
            TimeSpan expirestemp = new TimeSpan(0, 0, 0, 0, 0);
            HttpCookie encodedCookie = HttpSecureCookie.EncodeAes(new HttpCookie(key, value));
            if (HttpContext.Current.Response.Cookies[key] != null)
            {
                if (expirestemp != expires)
                {
                    HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.Add(expires);
                }
                HttpContext.Current.Response.Cookies[key].Value = encodedCookie.Value;
            }
            else
            {
                if (expirestemp != expires)
                {
                    encodedCookie.Expires = DateTime.Now.Add(expires);
                }
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }
        }

        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);
                value = decodedCookie.Value;
            }
            return value;
        }

        public static string GetCookieAes(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                HttpCookie decodedCookie = HttpSecureCookie.DecodeAes(cookie);
                value = decodedCookie.Value;
            }
            return value;
        }
        //public static void AesSetCookie(string value, TimeSpan expires, string domain)
        //{
        //    try
        //    {
        //        httpc
        //    }
        //    catch
        //    { }
        //}

        //public HttpResponseMessage Add()
        //{
        //    HttpRequestMessage request = new HttpRequestMessage();

        //    var response = new HttpResponseMessage();
        //    var Coki = new CookieHeaderValue("session-Id", "123");
        //    Coki.Expires = DateTimeOffset.Now.AddDays(2);
        //    Coki.Domain = request.RequestUri.Host;
        //    Coki.Path = "/";
        //    response.Headers.AddCookies(new CookieHeaderValue[] { Coki });

        //    return response;
        //}

        //public string Get()
        //{
        //    HttpRequestMessage request = new HttpRequestMessage();

        //    string ses_Id = "";
        //    CookieHeaderValue cookie = request.Headers.GetCookies("session-Id").FirstOrDefault();
        //    if (cookie != null)
        //    {
        //        ses_Id = cookie["session-Iid"].Value;
        //    }
        //    return ses_Id;
        //}
        //public HttpResponseMessage AddObject()
        //{
        //    var response = new HttpResponseMessage();
        //    var value = new NameValueCollection();
        //    value["sid"] = "123";
        //    value["token"] = "hij";
        //    value["theme"] = "green";
        //    var Coki = new CookieHeaderValue("session", value);
        //    response.Headers.AddCookies(new CookieHeaderValue[] { Coki });

        //       return response;
        //}

        //public void  GetObject()
        //{
        //    string ses_Id = "";
        //    string ses_Token = "";
        //    string theme = "";
        //    HttpRequestMessage request = new HttpRequestMessage();
        //    CookieHeaderValue Coki = request.Headers.GetCookies("session").FirstOrDefault();
        //    if (Coki != null)
        //    {
        //        CookieState cookie_State = Coki["session"];
        //        ses_Id = cookie_State["sid"];
        //        ses_Token = cookie_State["token"];
        //        theme = cookie_State["theme"];
        //    }
        //}

        public static void WriteLog(string err)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\log";
            Directory.CreateDirectory(path);
            FileInfo fileInfo = new FileInfo(path + "\\logErrors.txt");
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            var filename = path + "\\logErrors.txt";
            var sw = new System.IO.StreamWriter(filename, true);
            sw.WriteLine(DateTime.Now.ToString() + " " + err);
            sw.Close();
        }
    }
}
