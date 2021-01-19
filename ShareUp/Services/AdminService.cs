using System;
using System.Net;
using System.Linq;
using ShareUp.Models;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareUp.Services
{
    public class AdminService
    {
        public readonly IAdminSettings setup;

        public AdminService(IAdminSettings setup)
        {
            this.setup = setup;
        }

        /// <summary>
        /// Admin sends email messages to clients.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendEmail(string from, string to, string subject, string body)
        {
            int port = int.Parse(setup.port);
            SmtpClient host = new SmtpClient(setup.host, port);
            host.EnableSsl = true;

            host.UseDefaultCredentials = false;
            host.Credentials = new NetworkCredential(setup.client, setup.secret);
            MailMessage mail = new MailMessage();

            mail.To.Add(to);
            mail.From = new MailAddress(setup.client);
            mail.Subject = subject;

            mail.Body = body;
            mail.IsBodyHtml = true;
            host.Send(mail);
        }
    }
}
