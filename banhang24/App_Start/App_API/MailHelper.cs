using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace banhang24.App_Start.App_API
{
    public class MailHelper
    {
        public static void SendEmail(string emailid, string subject, string body)
        {
            MailMessage Msg = new MailMessage();
            Msg.Subject = subject;
            Msg.From = new MailAddress("noreply@open24.vn", "Open24.vn");
            Msg.To.Add(emailid);
            Msg.Body = body;
            Msg.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential networkCredential = new NetworkCredential("noreply@open24.vn", "Abc@12345678");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Port = 587;
            smtp.Send(Msg);
        }
    }
}