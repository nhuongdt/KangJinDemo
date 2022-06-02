using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace Open24.Hellper
{
    public class MailHellper
    {
        public static void SendThreadEmail (string emailid, string subject, string body)
        {
            Thread st = new Thread(() => SendEmail(emailid,subject,body));
            st.Start();
           
        }

        private static void SendEmail(string emailid, string subject, string body)
        {
            try
            {
                MailMessage Msg = new MailMessage();
                Msg.Subject = subject;
                Msg.From = new MailAddress(WebConfigurationManager.AppSettings["SPGmail"].ToString());
                Msg.To.Add(emailid);
                Msg.Body = body;
                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(WebConfigurationManager.AppSettings["SPGmail"].ToString(), WebConfigurationManager.AppSettings["SPPassWord"].ToString());
                //smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(Msg);
            }
            catch (Exception ex)
            {
                TelegramBot telegram = new TelegramBot();
                telegram.SendMessage(ex.Message);
            }
        }
    }
}