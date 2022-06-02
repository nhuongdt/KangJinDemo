using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Filters;

namespace banhang24.Compress
{
    public class DeflateCompressionAttribute : ActionFilterAttribute

    {
        public override void OnActionExecuted(HttpActionExecutedContext actContext)

        {
            var content = actContext.Response.Content;
            var bytes = content == null ? null : content.ReadAsByteArrayAsync().Result;
            var zlibbedContent = bytes == null ? new byte[0] :
            CompressionHelper.DeflateByte(bytes);

            actContext.Response.Content = new ByteArrayContent(zlibbedContent);
            actContext.Response.Content.Headers.Remove("Content-Type");
            actContext.Response.Content.Headers.Add("Content-encoding", "deflate");
            actContext.Response.Content.Headers.Add("Content-Type", "application/json");
            base.OnActionExecuted(actContext);
        }
    }

    public class CompressionHelper
    {
        public static byte[] DeflateByte(byte[] str)

        {
            if (str == null)
            {
                return null;
            }
            using (var output = new MemoryStream())
            {
                using (
                var compressor = new Ionic.Zlib.DeflateStream(
                output, Ionic.Zlib.CompressionMode.Compress,
                Ionic.Zlib.CompressionLevel.BestSpeed))
                {
                    compressor.Write(str, 0, str.Length);
                }
                return output.ToArray();
            }
        }
    }
}