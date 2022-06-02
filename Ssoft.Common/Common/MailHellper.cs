using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ssoft.Common.Common
{
    public class MailHellper
    {
        public static void SendThreadEmail(string emailAdress, string passAdress, string emailid, string subject, string body)
        {
            Thread st = new Thread(() => SendEmail(emailAdress, passAdress, emailid, subject, body));
            st.Start();

        }

        private static void SendEmail(string emailAdress, string passAdress, string emailid, string subject, string body)
        {
            MailMessage Msg = new MailMessage();
            Msg.Subject = subject;
            Msg.From = new MailAddress(emailAdress);
            Msg.To.Add(emailid);
            Msg.Body = body;
            Msg.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential networkCredential = new NetworkCredential(emailAdress, passAdress);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Port = 587;
            smtp.Send(Msg);
        }
    }
}
