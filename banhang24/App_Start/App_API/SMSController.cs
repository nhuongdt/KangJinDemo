using Model;
using Model.DAL;
using Model_banhang24vn;
using Model_banhang24vn.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace banhang24.App_Start.App_API
{
    public class SMSController
    {
        //const string APIKey = "4DAE81CC39F5FFB1AD0B9191E5D8E4";//Login to eSMS.vn to get this";//Dang ky tai khoan tai esms.vn de lay key//Register account at esms.vn to get key
        //const string SecretKey = "5BD0408955AB2B465F1D903F596429";//Login to eSMS.vn to get this";

        public static string SendGetRequest(string RequestUrl)
        {
            Uri address = new Uri(RequestUrl);
            HttpWebRequest request;
            HttpWebResponse response = null;
            StreamReader reader;
            if (address == null) { throw new ArgumentNullException("address"); }
            try
            {
                request = WebRequest.Create(address) as HttpWebRequest;
                request.UserAgent = ".NET Sample";
                request.KeepAlive = false;
                request.Timeout = 15 * 1000;
                response = request.GetResponse() as HttpWebResponse;
                if (request.HaveResponse == true && response != null)
                {
                    reader = new StreamReader(response.GetResponseStream());
                    string result = reader.ReadToEnd();
                    result = result.Replace("</string>", "");
                    return result;
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                    {
                        Console.WriteLine(
                            "The server returned '{0}' with the status code {1} ({2:d}).",
                            errorResponse.StatusDescription, errorResponse.StatusCode,
                            errorResponse.StatusCode);
                    }
                }
            }
            finally
            {
                if (response != null) { response.Close(); }
            }
            return null;
        }

        //Send SMS with Alpha Sender
        public static void SendBrandnameJson(HeThong_SMS model, Guid idbrandName)
        {
            //url vi du
            // sử dụng cách 1:
            RegisterServiceSm objBrand = new RegisterServiceSmsService().GetBrandNameById(idbrandName);
            SupplierSm objKey = new SupplierSmService().GetAPIKeyByIDSupllierSMS(objBrand.ID_SupplierSms.Value);
            string URL = "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_get?Phone=" + model.SoDienThoai + "&Content=" + model.NoiDung + "&Brandname=" + objBrand.Name + "&ApiKey=" + objKey.ApiKey + "&SecretKey=" + objKey.ApiSecret + "&IsUnicode=0&SmsType=2";

            string result = SendGetRequest(URL);
            JObject ojb = JObject.Parse(result);
            int CodeResult = (int)ojb["CodeResult"];//trả về 100 là thành công
            int CountRegenerate = (int)ojb["CountRegenerate"];
            string SMSID = (string)ojb["SMSID"];//id của tin nhắn
            model.ID = Guid.NewGuid();
            model.ThoiGianGui = DateTime.Now;
            model.TrangThai = CodeResult;
            model.ID_HoaDon = model.ID_HoaDon;
            model.GiaTien = (double)(objKey.Price * model.SoTinGui);
            model.IDTinNhan = SMSID;
            new HeThong_SMS_TinMauService().InsertTinNhan(model);
        }
    }
}

