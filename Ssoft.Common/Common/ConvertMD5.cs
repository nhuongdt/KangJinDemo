using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ssoft.Common.Common
{
   public class ConvertMD5
    {
        public static string GetMd5Hash(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] byteHash = md5.ComputeHash(byteValue);
            string strPassWordConnect = Convert.ToBase64String(byteHash);
            return strPassWordConnect.ToString();
        }
    }
}
