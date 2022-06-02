using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Model
{
    public class CookieProtectionHelperWrapper
    {
        private static MethodInfo _encode;
        private static MethodInfo _decode;
        /// <summary>
        /// Constructor
        /// </summary>
        static CookieProtectionHelperWrapper()
        {
            // obtaining a reference to System.Web assembly
            Assembly systemWeb = typeof(HttpContext).Assembly;
            if (systemWeb == null)
            {
                throw new InvalidOperationException("Unable to load System.Web.");
            }
            // obtaining a reference to the internal class CookieProtectionHelper
            Type cookieProtectionHelper = systemWeb.GetType("System.Web.Security.CookieProtectionHelper");
            if (cookieProtectionHelper == null)
            {
                throw new InvalidOperationException("Unable to get the internal class System.Web.Security.CookieProtectionHelper.");
            }
            // obtaining references to the methods of CookieProtectionHelper class
            _encode = cookieProtectionHelper.GetMethod("Encode", BindingFlags.NonPublic | BindingFlags.Static);
            _decode = cookieProtectionHelper.GetMethod("Decode", BindingFlags.NonPublic | BindingFlags.Static);
            if (_encode == null || _decode == null)
            {
                throw new InvalidOperationException("Unable to get the methods to invoke.");
            }
        }

        /// <summary>
        /// Wrapper arround CookieProtectionHelper.Encode
        /// </summary>
        /// <param name="cookieProtection">Protection Type</param>
        /// <param name="buf">Bytes buffer to encode</param>
        /// <param name="count">The number of bytes in the buffer</param>
        /// <returns>Encoded text</returns>
        public static string Encode(CookieProtection cookieProtection, byte[] buf, int count)
        {
            byte[] buf_return = MachineKey.Protect(buf, "SSOFTVN");
            return Convert.ToBase64String(buf_return);
        }

        /// <summary>
        /// Wrapper arround CookieProtectionHelper.Decode
        /// </summary>
        /// <param name="cookieProtection">Protection Type</param>
        /// <param name="data">String to decode</param>
        /// <returns>Decoded bytes</returns>
        public static byte[] Decode(CookieProtection cookieProtection, string data)
        {
            var bytes = Convert.FromBase64String(data);
            var output = MachineKey.Unprotect(bytes, "SSOFTVN");
            return output;
        }
    }
}
