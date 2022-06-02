using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZaloConnectApp
{
    public class CodeChallengeClass
    {
        public string genCodeVerifier()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 43)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string genCodeChallenge(string codeVerifier)
        {
            string result = null;
            try
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
                    //StringBuilder builder = new StringBuilder();
                    //for (int i = 0; i < bytes.Length; i++)
                    //{
                    //    builder.Append(bytes[i].ToString("x2"));
                    //}
                    //string hash256 = builder.ToString();
                    result = Convert.ToBase64String(bytes);
                }
            }
            catch
            {

            }
            return result;
        }
    }
}
