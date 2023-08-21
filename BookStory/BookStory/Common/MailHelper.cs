using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace BookStory.Common
{
    public class MailHelper
    {
        public void SendMail(string toEmail, string subject, string content)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string From = config.GetSection("MailInfo")["MailAddress"];
            string DisplayAddress = config.GetSection("MailInfo")["DisplayName"];
            string Password = config.GetSection("MailInfo")["Password"];
            string Port = config.GetSection("MailInfo")["Port"];
            string Host = config.GetSection("MailInfo")["Host"];
            MailMessage Msg = new MailMessage();
            Msg.From = new MailAddress(From, DisplayAddress);// replace with valid value
            Msg.Subject = subject;
            Msg.To.Add(toEmail); //replace with correct values
            Msg.Body = content;
            Msg.IsBodyHtml = true;
            Msg.Priority = MailPriority.High;

            using (SmtpClient smtp = new SmtpClient(Host, Convert.ToInt32(Port)))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(From, Password);// replace with valid value
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(Msg);
            }
        }
    }
}
