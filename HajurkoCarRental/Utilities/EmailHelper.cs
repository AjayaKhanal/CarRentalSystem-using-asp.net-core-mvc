using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace HajurkoCarRental.Utilities
{
    public class EmailHelper
    {
        public static async void SendEmail(String title, String body, List<string> emails)
        {
            //var mail = "liar.t.imus56600@gmail.com";
            //var pwd = "mzranmhhukeigxrn";
            var mail = "ajayakhanal957@gmail.com";
            var pwd = "ajaya@2060";

            try { 
            // create an instance of SmtpClient
            using (SmtpClient client = new SmtpClient())
            {
                // configure the SMTP client with the necessary settings
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(mail, pwd);

                // create a new MailMessage object
                MailMessage message = new MailMessage();
                message.From = new MailAddress(mail);

				message.To.Add(string.Join(",", emails));

                message.Subject = title;
                message.Body = body;


                // send the email
                await client.SendMailAsync(message);
            }}
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }
}
