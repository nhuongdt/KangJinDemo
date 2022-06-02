using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ZaloConnectApp
{
    public class CZaloApi
    {
        public ZaloApiToKen GetToken(string code, string codeverifier)
        {
            ZaloApiToKen result = new ZaloApiToKen();
            using (HttpClient httpClient = new HttpClient())
            {
                //httpClient.BaseAddress = new Uri("https://oauth.zaloapp.com/v4/oa/access_token");
                string url = "https://oauth.zaloapp.com/v4/oa/access_token";
                //httpClient.BaseAddress = new Uri("https://localhost:44309/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                ConfigZalo zaloConfig = new ConfigZalo();
                zaloConfig = ReadXml();
                if (zaloConfig.secret_key != "")
                {
                    httpClient.DefaultRequestHeaders.Add("secret_key", zaloConfig.secret_key);
                    
                    List<KeyValuePair<string, string>> lstKey = new List<KeyValuePair<string, string>>();
                    lstKey.Add(new KeyValuePair<string, string>("code", code));
                    lstKey.Add(new KeyValuePair<string, string>("app_id", zaloConfig.app_id));
                    lstKey.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new FormUrlEncodedContent(lstKey)
                    };

                    //var response = httpClient.PostAsync(url, data).GetAwaiter().GetResult();
                    var response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                    string apiResult = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    result = JsonConvert.DeserializeObject<ZaloApiToKen>(apiResult);
                }
            }
            return result;
        }

        public ZaloOfficialAccountInfoApiResult GetZaloOfficialAccountInfoApiResult(string accessToken)
        {
            ZaloOfficialAccountInfoApiResult result = new ZaloOfficialAccountInfoApiResult();
            using (HttpClient httpClient = new HttpClient())
            {
                string url = "https://openapi.zalo.me/v2.0/oa/getoa";
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                ConfigZalo zaloConfig = new ConfigZalo();
                zaloConfig = ReadXml();
                if (zaloConfig.secret_key != "")
                {
                    httpClient.DefaultRequestHeaders.Add("access_token", accessToken);

                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                    //var response = httpClient.PostAsync(url, data).GetAwaiter().GetResult();
                    var response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                    string apiResult = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    result = JsonConvert.DeserializeObject<ZaloOfficialAccountInfoApiResult>(apiResult);
                }
            }
            return result;
        }

        public ZaloApiToKen GetAccessTokenFromRefreshToken(string refreshToken)
        {
            ZaloApiToKen result = new ZaloApiToKen();
            using (HttpClient httpClient = new HttpClient())
            {
                string url = "https://oauth.zaloapp.com/v4/oa/access_token";
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                ConfigZalo zaloConfig = new ConfigZalo();
                zaloConfig = ReadXml();
                if (zaloConfig.secret_key != "")
                {
                    httpClient.DefaultRequestHeaders.Add("secret_key", zaloConfig.secret_key);
                    
                    List<KeyValuePair<string, string>> lstKey = new List<KeyValuePair<string, string>>();
                    lstKey.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
                    lstKey.Add(new KeyValuePair<string, string>("app_id", zaloConfig.app_id));
                    lstKey.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new FormUrlEncodedContent(lstKey)
                    };
                    var response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                    string apiResult = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    result = JsonConvert.DeserializeObject<ZaloApiToKen>(apiResult);
                }
            }    
            return result;
        }

        public ConfigZalo ReadXml()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "config.xml";
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(ConfigZalo));
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                ConfigZalo overview = (ConfigZalo)reader.Deserialize(file);
                file.Close();
                return overview;
            }
            catch
            {
                return new ConfigZalo();
            }
        }

        public class ConfigZalo
        {
            public string secret_key { get; set; } = "";
            public string app_id { get; set; } = "";
        }

        public class ZaloApiToKen
        {
            public string access_token { get; set; } = "";
            public string refresh_token { get; set; } = "";
            public string expires_in { get; set; } = "";
        }
        public class ZaloOfficialAccountInfo
        {
            public long oa_id { get; set; } = 0;
            public string description { get; set; } = "";
            public string name { get; set; } = "";
            public string avatar { get; set; } = "";
            public string cover { get; set; } = "";
            public bool is_verified { get; set; } = false;
        }

        public class ZaloOfficialAccountInfoApiResult
        {
            public int error { get; set; }
            public string message { get; set; }
            public ZaloOfficialAccountInfo data { get; set; }
        }
    }
}
