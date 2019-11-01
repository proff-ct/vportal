using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace VisibilityPortal_BLL.Services.Messaging
{
  public class EmailService : IIdentityMessageService
  {
      private SmtpClient _smtpClient;

      public Task SendAsync(IdentityMessage message)
        {
            _smtpClient = new SmtpClient
            {
                Host = GMAILConfig.host,
                Port = GMAILConfig.port,
                EnableSsl = GMAILConfig.GmailSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(GMAILConfig.username, GMAILConfig.password)
            };


            MailMessage email = new MailMessage(GMAILConfig.username, message.Destination)
            {
                Subject = message.Subject, Body = message.Body, IsBodyHtml = true, Priority = MailPriority.High
            };

            return _smtpClient.SendMailAsync(email);

           
    }
  }
}
