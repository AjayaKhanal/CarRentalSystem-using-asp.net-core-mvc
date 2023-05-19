using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace HajurkoCarRental.Utilities
{
    public class EmailHelper
    {
        public static async void SendEmail(String title, String body, List<string> emails)
        {
            // create an instance of SmtpClient
            using (SmtpClient client = new SmtpClient())
            {
                // configure the SMTP client with the necessary settings
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("liar.t.imus56600@gmail.com", "mzranmhhukeigxrn");

                // create a new MailMessage object
                MailMessage message = new MailMessage();
                message.From = new MailAddress("liar.t.imus56600@gmail.com");

				message.To.Add(string.Join(",", emails));

                message.Subject = title;
                message.Body = body;


                // send the email
                await client.SendMailAsync(message);
            }

        }

    }
}
