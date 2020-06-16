using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace InReachProject.Services
{

    /*Service for sending email with URL to user
     * */
    public class MailService
    {
        private readonly IConfiguration _configure;
        public MailService(IConfiguration configure)
        {
            _configure = configure;
        }

        //Email account from which email will be sent
        private string fromEmail = "SeanMcQueenInReachProject@gmail.com";

        public void SendMail(string email, string url)
        {
           
            //Compose email
            var fromAddress = new MailAddress(fromEmail, "Sean McQueen");
            var toAddress = new MailAddress(email, "InReach");
            string fromPassword = _configure["emailPassword"];
            string subject = "Here is your S3 URL!";
            string body = "Hello, \nThank you for uploading a file. " +
                          "This URL will expire in 1 hour. \n" +
                            url +
                            "\nBest, \nSean McQueen";

            //Set up SMTP settings
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            //Create message
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                //send message
                smtp.Send(message);
            }
        }


    }
}
