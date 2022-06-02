
using Model_banhang24vn.CustomView;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Model_banhang24vn.Common
{

    public class FlatFileAccess
    {
        public static Dictionary<string, string> Read301CSV()
        {
            string file = "App_Data/301.csv";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), file);

            if (File.Exists(path))
            {
                using (TextReader sr = new StreamReader(path))
                {
                    Dictionary<string, string> redirect_dict = new Dictionary<string, string>();
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',');
                        redirect_dict.Add(columns[0], columns[1]);
                    }
                    return redirect_dict;
                }
            }
            else
                return null;

        }

    }
    public class StaticVariable
    {
        /// <summary>
        /// Kiểm tra địa chỉ email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>
        /// True: đúng định dạng email
        /// </returns>
        public static bool EmailIsValid(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// cắt lấy chữ đầu tiên của 2 từ đầu tiên làm mã
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetCharFirst(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                char[] whitespace = new char[] { ' ', '\t' };
                string[] lines = input.Split(whitespace).ToList()
                    .Where(o=>!string.IsNullOrWhiteSpace(o)).Take(2)
                   .Select(o=>o.Substring(0,1).ToUpper())
                    .ToArray();
                return string.Join("", lines);
            }
            return string.Empty;
        }
        /// <summary>
        /// cắt lấy chữ đầu tiên của  làm mã
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetCharInput(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                char[] whitespace = new char[] { ' ', '\t' };
                string[] lines = input.Split(whitespace).ToList()
                    .Where(o => !string.IsNullOrWhiteSpace(o))
                   .Select(o => o.Substring(0, 1).ToUpper())
                    .ToArray();
                return string.Join("", lines);
            }
            return string.Empty;
        }

        /// <summary>
        /// convert sang định dạng VND
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string ConvertVND(int price)
        {
          return  String.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:c}",price);
        }

        /// <summary>
        /// Kiểm tra địa chỉ phone
        /// </summary>
        /// <param name="email"></param>
        /// <returns>
        /// True: đúng định dạng phone
        /// </returns>
        public static bool PhoneIsValid(string phone)
        {
            Regex pattern = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return pattern.IsMatch(phone);
        }


        /// <summary>
        /// Dãy tiếng việt có dấu cần chuyển đổi
        /// </summary>
        public static readonly string[] VietnameseSigns = new string[]

      { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
        "đ",
        "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
        "í","ì","ỉ","ĩ","ị",
        "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
        "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
        "ý","ỳ","ỷ","ỹ","ỵ",};

        /// <summary>
        /// Dãy tiếng việt ko dấu cần chuyển đổi
        /// </summary>
        public static readonly string[] EnglishSigns = new string[]
        { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};

        /// <summary>
        /// chuyển tiếng việt có dấu sang không dấu
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSign4VietnameseString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            for (int i = 0; i < VietnameseSigns.Length; i++)
            {
                str = str.Replace(VietnameseSigns[i], EnglishSigns[i]);
                str = str.Replace(VietnameseSigns[i].ToUpper(), EnglishSigns[i].ToUpper());
            }
            return str;

        }

        /// <summary>
        /// Convert title to url
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string ConvetTitleToUrl(string title)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(title))
            {
                result = RemoveSign4VietnameseString(title.ToLower());
                result = Regex.Replace(result, @"[^\w\d]", "-");
                result = string.Join("-", result.Split('-').Where(o => !string.IsNullOrWhiteSpace(o)));
            }
            return result;
        }

        /// <summary>
        ///  Tính tỷ lệ %
        /// </summary>
        /// <param name="input"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static double RateCalculation(double input, int count)
        {
            return Math.Round((input / count) * 100, 2);
        }

        /// <summary>
        /// Kiểm tra thiết bị truy cập
        /// </summary>
        /// <param name="ua"></param>
        /// <returns></returns>
        public static int GetDeviceType(string ua)
        {
            // Check if user agent is a smart TV - http://goo.gl/FocDk
            if (Regex.IsMatch(ua, @"GoogleTV|SmartTV|Internet.TV|NetCast|NETTV|AppleTV|boxee|Kylo|Roku|DLNADOC|CE\-HTML", RegexOptions.IgnoreCase))
            {
                return (int)Notification.Device.tv;
            }
            // Check if user agent is a TV Based Gaming Console
            else if (Regex.IsMatch(ua, "Xbox|PLAYSTATION.3|Wii", RegexOptions.IgnoreCase))
            {
                return (int)Notification.Device.tv;
            }
            // Check if user agent is a Tablet
            else if ((Regex.IsMatch(ua, "iP(a|ro)d", RegexOptions.IgnoreCase) || (Regex.IsMatch(ua, "tablet", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "RX-34", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "FOLIO", RegexOptions.IgnoreCase))))
            {
                return (int)Notification.Device.tablet;
            }
            // Check if user agent is an Android Tablet
            else if ((Regex.IsMatch(ua, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Android", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Fennec|mobi|HTC.Magic|HTCX06HT|Nexus.One|SC-02B|fone.945", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.tablet;
            }
            // Check if user agent is a Kindle or Kindle Fire
            else if ((Regex.IsMatch(ua, "Kindle", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "Mac.OS", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Silk", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.tablet;
            }
            // Check if user agent is a pre Android 3.0 Tablet
            else if ((Regex.IsMatch(ua, @"GT-P10|SC-01C|SHW-M180S|SGH-T849|SCH-I800|SHW-M180L|SPH-P100|SGH-I987|zt180|HTC(.Flyer|\\_Flyer)|Sprint.ATP51|ViewPad7|pandigital(sprnova|nova)|Ideos.S7|Dell.Streak.7|Advent.Vega|A101IT|A70BHT|MID7015|Next2|nook", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "MB511", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "RUTEM", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.tablet;
            }
            // Check if user agent is unique Mobile User Agent
            else if ((Regex.IsMatch(ua, "BOLT|Fennec|Iris|Maemo|Minimo|Mobi|mowser|NetFront|Novarra|Prism|RX-34|Skyfire|Tear|XV6875|XV6975|Google.Wireless.Transcoder", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.mobile;
            }
            // Check if user agent is an odd Opera User Agent - http://goo.gl/nK90K
            else if ((Regex.IsMatch(ua, "Opera", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Windows.NT.5", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, @"HTC|Xda|Mini|Vario|SAMSUNG\-GT\-i8000|SAMSUNG\-SGH\-i9", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.mobile;
            }
            // Check if user agent is Windows Desktop
            else if ((Regex.IsMatch(ua, "Windows.(NT|XP|ME|9)")) && (!Regex.IsMatch(ua, "Phone", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "Win(9|.9|NT)", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.desktop;
            }
            // Check if agent is Mac Desktop
            else if ((Regex.IsMatch(ua, "Macintosh|PowerPC", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Silk", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.desktop;
            }
            // Check if user agent is a Linux Desktop
            else if ((Regex.IsMatch(ua, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "X11", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.desktop;
            }
            // Check if user agent is a Solaris, SunOS, BSD Desktop
            else if ((Regex.IsMatch(ua, "Solaris|SunOS|BSD", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.desktop;
            }
            // Check if user agent is a Desktop BOT/Crawler/Spider
            else if ((Regex.IsMatch(ua, "Bot|Crawler|Spider|Yahoo|ia_archiver|Covario-IDS|findlinks|DataparkSearch|larbin|Mediapartners-Google|NG-Search|Snappy|Teoma|Jeeves|TinEye", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Mobile", RegexOptions.IgnoreCase)))
            {
                return (int)Notification.Device.desktop;
            }
            // Otherwise assume it is a Mobile Device
            else
            {
                return (int)Notification.Device.mobile;
            }
        }
        /// <summary>
        /// Kiểm tra thiết bị truy cập Lây ra tên
        /// </summary>
        /// <param name="ua"></param>
        /// <returns></returns>
        public static string  GetNameDeviceType(string ua)
        {
            // Check if user agent is a smart TV - http://goo.gl/FocDk
            if (Regex.IsMatch(ua, @"GoogleTV|SmartTV|Internet.TV|NetCast|NETTV|AppleTV|boxee|Kylo|Roku|DLNADOC|CE\-HTML", RegexOptions.IgnoreCase))
            {
                return "SmartTV";
            }
            // Check if user agent is a TV Based Gaming Console
            else if (Regex.IsMatch(ua, "Xbox|PLAYSTATION.3|Wii", RegexOptions.IgnoreCase))
            {
                return "SmartTV";
            }
            // Check if user agent is a Tablet
            else if ((Regex.IsMatch(ua, "iP(a|ro)d", RegexOptions.IgnoreCase) || (Regex.IsMatch(ua, "tablet", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "RX-34", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "FOLIO", RegexOptions.IgnoreCase))))
            {
                return "tablet";
            }
            // Check if user agent is an Android Tablet
            else if ((Regex.IsMatch(ua, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Android", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Fennec|mobi|HTC.Magic|HTCX06HT|Nexus.One|SC-02B|fone.945", RegexOptions.IgnoreCase)))
            {
                return "tablet";
            }
            // Check if user agent is a Kindle or Kindle Fire
            else if ((Regex.IsMatch(ua, "Kindle", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "Mac.OS", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Silk", RegexOptions.IgnoreCase)))
            {
                return "tablet";
            }
            // Check if user agent is a pre Android 3.0 Tablet
            else if ((Regex.IsMatch(ua, @"GT-P10|SC-01C|SHW-M180S|SGH-T849|SCH-I800|SHW-M180L|SPH-P100|SGH-I987|zt180|HTC(.Flyer|\\_Flyer)|Sprint.ATP51|ViewPad7|pandigital(sprnova|nova)|Ideos.S7|Dell.Streak.7|Advent.Vega|A101IT|A70BHT|MID7015|Next2|nook", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "MB511", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "RUTEM", RegexOptions.IgnoreCase)))
            {
                return "tablet";
            }
            // Check if user agent is unique Mobile User Agent
            else if ((Regex.IsMatch(ua, "BOLT|Fennec|Iris|Maemo|Minimo|Mobi|mowser|NetFront|Novarra|Prism|RX-34|Skyfire|Tear|XV6875|XV6975|Google.Wireless.Transcoder", RegexOptions.IgnoreCase)))
            {
                return "mobile";
            }
            // Check if user agent is an odd Opera User Agent - http://goo.gl/nK90K
            else if ((Regex.IsMatch(ua, "Opera", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "Windows.NT.5", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, @"HTC|Xda|Mini|Vario|SAMSUNG\-GT\-i8000|SAMSUNG\-SGH\-i9", RegexOptions.IgnoreCase)))
            {
                return "mobile";
            }
            // Check if user agent is Windows Desktop
            else if ((Regex.IsMatch(ua, "Windows.(NT|XP|ME|9)")) && (!Regex.IsMatch(ua, "Phone", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ua, "Win(9|.9|NT)", RegexOptions.IgnoreCase)))
            {
                return "desktop";
            }
            // Check if agent is Mac Desktop
            else if ((Regex.IsMatch(ua, "Macintosh|PowerPC", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Silk", RegexOptions.IgnoreCase)))
            {
                return "desktop";
            }
            // Check if user agent is a Linux Desktop
            else if ((Regex.IsMatch(ua, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(ua, "X11", RegexOptions.IgnoreCase)))
            {
                return "desktop";
            }
            // Check if user agent is a Solaris, SunOS, BSD Desktop
            else if ((Regex.IsMatch(ua, "Solaris|SunOS|BSD", RegexOptions.IgnoreCase)))
            {
                return "desktop";
            }
            // Check if user agent is a Desktop BOT/Crawler/Spider
            else if ((Regex.IsMatch(ua, "Bot|Crawler|Spider|Yahoo|ia_archiver|Covario-IDS|findlinks|DataparkSearch|larbin|Mediapartners-Google|NG-Search|Snappy|Teoma|Jeeves|TinEye", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(ua, "Mobile", RegexOptions.IgnoreCase)))
            {
                return "desktop";
            }
            // Otherwise assume it is a Mobile Device
            else
            {
                return "mobile";
            }
        }

        /// <summary>
        /// Kiểm tra hệ điều hành truy cập
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string CheckSystem(string request)
        {
            if (request.Contains("Android"))
            {
                return "Android";
            }
            else if (request.Contains("iPad") || request.Contains("iPhone") || request.Contains("Mac OS"))
            {
                return "MAC_OS";
            }
            else if (request.Contains("Linux") && request.Contains("KFAPWI"))
            {
                return "Linux";
            }
            else if (request.Contains("Windows NT"))
            {
                return "Windows";
            }
            else
            {
                return "Other";
            }
        }

        /// <summary>
        /// Convert Tags
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUnsignString(string input)
        {
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            input = input.Replace(".", "-");
            input = input.Replace(" ", "-");
            input = input.Replace(",", "-");
            input = input.Replace(";", "-");
            input = input.Replace(":", "-");
            input = input.Replace("  ", "-");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            while (str2.Contains("--"))
            {
                str2 = str2.Replace("--", "-").ToLower();
            }
            return str2;
        }

        public static string GetSitemapDocument(List<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-dd")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }

        public  static void deletefile(string file)
        {
            var path = Path.Combine(HttpContext.Current.Server.MapPath(file));
            if (System.IO.File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
  
    public class SitemapNode
    {
        public SitemapFrequency? Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }
       
}
