using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Model_banhang24vn.Common
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

        //// Verify a hash against a string.
        //public static bool VerifyMd5Hash(string input, string hash)
        //{
        //    using (MD5 md5Hash = MD5.Create())
        //    {
        //        if (string.IsNullOrWhiteSpace(input) && string.IsNullOrWhiteSpace(hash)) return true;
        //        if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(hash)) return false;
        //        // Hash the input.
        //        string hashOfInput = GetMd5Hash(input);

        //        // Create a StringComparer an compare the hashes.
        //        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        //        if (0 == comparer.Compare(hashOfInput, hash))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}
    }
}
