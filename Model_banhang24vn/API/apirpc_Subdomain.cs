using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Model_banhang24vn
{
    public class apirpc_Subdomain
    {
        #region add new
        public static string AddNewSubdomain(string strSubdomain)
        {
            try
            {
                Request request = new Request();
                //
                XmlDocument Paket_subdomain = CreateFile_AddNewSubdomain(request, strSubdomain);
                //
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidation);
                try
                {
                    XmlDocument result = request.Send(Paket_subdomain);
                    PrintResult(result);
                    //
                    string strReturn = GetReturnSendRequest_AddNewSubdomain(result);
                    return strReturn.Trim();
                }
                catch (Exception e)
                {
                    return string.Format("Request error: {0}", e.Message);
                }
            }
            catch (Exception e)
            {
                return string.Format("Request error: {0}", e.Message);
            }
        }

        static XmlDocument CreateFile_AddNewSubdomain(Request rq, string strSubdomain)
        {
            XmlDocument doc = new XmlDocument();
            //(1) the xml declaration is recommended, but not mandatory
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            //(2) string.Empty makes cleaner code
            XmlElement element1 = doc.CreateElement(string.Empty, "packet", string.Empty);
            doc.AppendChild(element1);

            XmlElement element2 = doc.CreateElement(string.Empty, "subdomain", string.Empty);
            element1.AppendChild(element2);

            XmlElement element3 = doc.CreateElement(string.Empty, "add", string.Empty);
            element2.AppendChild(element3);

            // <parent>ssoftvn.com</parent>
            //< name >Subdomain  </ name >
            string strDomain_parent = rq.Hostname;
            XmlElement element3_1 = doc.CreateElement(string.Empty, "parent", string.Empty);
            XmlText value3_1 = doc.CreateTextNode(strDomain_parent);// "open24.vn");
            element3_1.AppendChild(value3_1);
            element3.AppendChild(element3_1);

            XmlElement element3_2 = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText value3_2 = doc.CreateTextNode(strSubdomain);
            element3_2.AppendChild(value3_2);
            element3.AppendChild(element3_2);

            //
            //       < property >
            //  < name > www_root </ name >
            //  < value >/ httpdocs </ value >
            XmlElement element3_3 = doc.CreateElement(string.Empty, "property", string.Empty);
            element3.AppendChild(element3_3);

            XmlElement element3_3_1 = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText value3_3_1 = doc.CreateTextNode("www_root");
            element3_3_1.AppendChild(value3_3_1);
            element3_3.AppendChild(element3_3_1);

            XmlElement element3_3_2 = doc.CreateElement(string.Empty, "value", string.Empty);
            XmlText value3_3_2 = doc.CreateTextNode("/subdomain");
            element3_3_2.AppendChild(value3_3_2);
            element3_3.AppendChild(element3_3_2);
            //</ property >

            //  < property >
            //  < name > ftp_login </ name >
            //  < value > nhssos1v </ value >
            XmlElement element3_4 = doc.CreateElement(string.Empty, "property", string.Empty);
            element3.AppendChild(element3_4);

            XmlElement element3_4_1 = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText value3_4_1 = doc.CreateTextNode("ftp_login");
            element3_4_1.AppendChild(value3_4_1);
            element3_4.AppendChild(element3_4_1);

            string value_ftp_login = rq.Login;
            XmlElement element3_4_2 = doc.CreateElement(string.Empty, "value", string.Empty);
            XmlText value3_4_2 = doc.CreateTextNode(value_ftp_login);// ("nhpha4vw");
            element3_4_2.AppendChild(value3_4_2);
            element3_4.AppendChild(element3_4_2);
            //</ property >
            //
            //  < property >
            //  < name > ftp_password </ name >
            //  < value > Ssoftvn20162020 </ value >
            XmlElement element3_5 = doc.CreateElement(string.Empty, "property", string.Empty);
            element3.AppendChild(element3_5);

            XmlElement element3_5_1 = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText value3_5_1 = doc.CreateTextNode("ftp_password");
            element3_5_1.AppendChild(value3_5_1);
            element3_5.AppendChild(element3_5_1);

            string value_ftp_password = rq.Password;
            XmlElement element3_5_2 = doc.CreateElement(string.Empty, "value", string.Empty);
            XmlText value3_5_2 = doc.CreateTextNode(value_ftp_password);// ("Ssoftvn2015");
            element3_5_2.AppendChild(value3_5_2);
            element3_5.AppendChild(element3_5_2);
            //</ property >
            //
            //  < property >
            //  < name > ssi </ name >
            //  < value > true </ value >
            //</ property >
            XmlElement element3_6 = doc.CreateElement(string.Empty, "property", string.Empty);
            element3.AppendChild(element3_6);

            XmlElement element3_6_1 = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText value3_6_1 = doc.CreateTextNode("ssi");
            element3_6_1.AppendChild(value3_6_1);
            element3_6.AppendChild(element3_6_1);

            XmlElement element3_6_2 = doc.CreateElement(string.Empty, "value", string.Empty);
            XmlText value3_6_2 = doc.CreateTextNode("true");
            element3_6_2.AppendChild(value3_6_2);
            element3_6.AppendChild(element3_6_2);
            //</ property >
            //
            //  < property >
            //  < name > ssi_html </ name >
            //  < value > true </ value >
            //</ property >
            XmlElement element3_7 = doc.CreateElement(string.Empty, "property", string.Empty);
            element3.AppendChild(element3_7);

            XmlElement element3_7_1 = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText value3_7_1 = doc.CreateTextNode("ssi_html");
            element3_7_1.AppendChild(value3_7_1);
            element3_7.AppendChild(element3_7_1);

            XmlElement element3_7_2 = doc.CreateElement(string.Empty, "value", string.Empty);
            XmlText value3_7_2 = doc.CreateTextNode("true");
            element3_7_2.AppendChild(value3_7_2);
            element3_7.AppendChild(element3_7_2);
            //</ property >


            return doc;
        }
        /*
         * Response Samples
        A positive response received from the server upon subdomain creation can look as follows:
        <packet version="1.5.2.0">
        <subdomain>
           <add>
              <result>
                 <status>ok</status>
                 <id>1</id>
              </result>
           </add>
        </subdomain>
        </packet>
        //
        A negative response can look as follows:
        //
        <packet version="1.5.2.0">
        <subdomain>
           <add>
              <result>
                 <status>error</status>
                 <errcode>1034</errcode>
                 <errtext>subdomains are not supported on sites without physical hosting</errtext>
              </result>
           </add>
        </subdomain>
        </packet>
         */
        static string GetReturnSendRequest_AddNewSubdomain(XmlDocument result)
        {
            string strReturn = "";
            XmlNodeList xmlnode_succeeds = result.GetElementsByTagName("id");
            if (xmlnode_succeeds != null && xmlnode_succeeds.Count > 0)
            {
                return strReturn;
            }
            else
            {
                XmlNodeList xmlnode = result.GetElementsByTagName("result");
                for (int i = 0; i <= xmlnode.Count - 1; i++)
                {
                    xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                    strReturn = xmlnode[i].ChildNodes.Item(0).InnerText.Trim() + " - " + xmlnode[i].ChildNodes.Item(1).InnerText.Trim() + " -  " + xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                }
            }
            return strReturn;
        }
        #endregion

        #region delete
        public static string DeleteSubdomain(string strSubdomain)
        {
            try
            {
                Request request = new Request();
                //
                XmlDocument Paket_subdomain = CreateFile_DeleteSubdomain(request, strSubdomain);
                //
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidation);
                try
                {
                    XmlDocument result = request.Send(Paket_subdomain);
                    PrintResult(result);
                    //
                    string strReturn = GetReturnSendRequest_DeleteSubdomain(result);
                    return strReturn.Trim();
                }
                catch (Exception e)
                {
                    return string.Format("Request error: {0}", e.Message);
                }
            }
            catch (Exception e)
            {
                return string.Format("Request error: {0}", e.Message);
            }
        }
        static XmlDocument CreateFile_DeleteSubdomain(Request rq, string strSubdomain)
        {
            XmlDocument file_Del = new XmlDocument();
            //< packet >
            // < subdomain >
            //  < del >
            //   < filter >
            //      < name > forum.example.com </ name >
            //   </ filter >
            //  </ del >
            // </ subdomain >
            //</ packet >

            //(1) the xml declaration is recommended, but not mandatory
            XmlDeclaration xmlDeclaration = file_Del.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = file_Del.DocumentElement;
            file_Del.InsertBefore(xmlDeclaration, root);

            //(2) string.Empty makes cleaner code
            XmlElement node_packet = file_Del.CreateElement(string.Empty, "packet", string.Empty);
            file_Del.AppendChild(node_packet);
            //
            XmlElement node_subdomain = file_Del.CreateElement(string.Empty, "subdomain", string.Empty);
            node_packet.AppendChild(node_subdomain);
            //
            XmlElement node_del = file_Del.CreateElement(string.Empty, "del", string.Empty);
            node_subdomain.AppendChild(node_del);
            //
            XmlElement node_filter = file_Del.CreateElement(string.Empty, "filter", string.Empty);
            node_del.AppendChild(node_filter);
            //
            string valueSubdomainDel = strSubdomain + "." + rq.Hostname;
            XmlElement node_filter_name1 = file_Del.CreateElement(string.Empty, "name", string.Empty);
            XmlText value_filter_name1 = file_Del.CreateTextNode(valueSubdomainDel);
            node_filter_name1.AppendChild(value_filter_name1);
            //
            node_filter.AppendChild(node_filter_name1);
            //
            return file_Del;
        }
        /*
           <packet version="1.5.2.0">
            <subdomain>
               <del>
                  <result>
                     <status>ok</status>
                     <filter-id>13</filter-id>
                     <id>13</id>
                  </result>
               </del>
            </subdomain>
            </packet>
            //
            <packet version="1.5.2.0">
            <subdomain>
               <del>
                  <result>
                     <status>error</status>
                     <errcode>1013</errcode>
                     <errtext>subdomain does not exist</errtext>
                     <filter-id>forum</filter-id>
                  </result>
               </del>
            </subdomain>
           </packet>
         */
        static string GetReturnSendRequest_DeleteSubdomain(XmlDocument result)
        {
            string strReturn = "";
            XmlNodeList xmlnode_succeeds = result.GetElementsByTagName("id");
            if (xmlnode_succeeds != null && xmlnode_succeeds.Count > 0)
            {
                return strReturn;
            }
            else
            {
                XmlNodeList xmlnode = result.GetElementsByTagName("result");
                for (int i = 0; i <= xmlnode.Count - 1; i++)
                {
                    xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                    strReturn = xmlnode[i].ChildNodes.Item(0).InnerText.Trim() + " - " + xmlnode[i].ChildNodes.Item(1).InnerText.Trim() + " -  " + xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                }
            }
            return strReturn;
        }
        #endregion

        #region update
        public static void UpdateSubdomain(string strSubdomain)
        {
        }
        static XmlDocument CreateFile_UpdateSubdomain(string strSubdomain)
        {
            XmlDocument file_Upd = new XmlDocument();

            return file_Upd;
        }

        #endregion

        #region ###
        private static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors != SslPolicyErrors.RemoteCertificateNotAvailable)
                return true;
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        private static void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            Console.WriteLine("Validation error: {0}", e.Message);
        }

        static void PrintResult(XmlDocument document)
        {
            XmlTextWriter writer = new XmlTextWriter(Console.Out);
            writer.Formatting = Formatting.Indented;

            document.WriteTo(writer);

            writer.Flush();
            Console.WriteLine();
        }
        #endregion
    }

    public class Request
    {
        public string Hostname = "open24.vn";
        public string Login = "admin";
        public string Password = "Ssoftvn20172017";

        //public string Hostname = "ssoftvn.com";
        //public string Login = "nhssos1v";
        //public string Password = "Ssoftvn20162020";
        public Request()
        {
        }
        public string AgentEntryPoint { get { return "https://" + Hostname + ":8443/enterprise/control/agent.php"; } }
        public XmlDocument Send(XmlDocument packet)
        {
            HttpWebRequest request = SendRequest(packet.OuterXml);
            XmlDocument result = GetResponse(request);
            return result;
        }

        public XmlDocument Send(Stream packet)
        {
            using (TextReader reader = new StreamReader(packet))
            {
                return Send(Parse(reader));
            }
        }

        public XmlDocument Send(string packetUri)
        {
            using (TextReader reader = new StreamReader(packetUri))
            {
                return Send(Parse(reader));
            }
        }

        private HttpWebRequest SendRequest(string message)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AgentEntryPoint);

            request.Method = "POST";
            request.Headers.Add("HTTP_AUTH_LOGIN", Login);
            request.Headers.Add("HTTP_AUTH_PASSWD", Password);
            request.ContentType = "text/xml";
            request.ContentLength = message.Length;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] buffer = encoding.GetBytes(message);

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(buffer, 0, message.Length);
            }
            return request;
        }

        private XmlDocument Parse(TextReader xml)
        {
            XmlDocument document = new XmlDocument();
            using (XmlReader reader = XmlTextReader.Create(xml))
            {
                document.Load(reader);
            }
            return document;
        }

        private XmlDocument GetResponse(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (TextReader reader = new StreamReader(stream))
            {
                return Parse(reader);
            }
        }

        public static object CreateResponse(HttpStatusCode internalServerError, string v)
        {
            throw new NotImplementedException();
        }
    }
}
