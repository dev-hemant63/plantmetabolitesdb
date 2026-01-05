using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace PlantMetabolitesDB.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            try
            {
                // Replace these values with your Gmail credentials
                string fromEmail = "tmsdatabase80@gmail.com";
                string fromPassword = "ajbujtzhmoztcgbv";   //"TMS@db80cdri"; // Use App Password or your Gmail account password
                string toEmail = "aftabjafri06@gmail.com";
                string subject = "Subject of your email";
                string body = "Body of your email";

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true, // You can set this to false if your email body is plain text.
                };

                smtpClient.Send(mailMessage);

                ViewBag.Message = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error sending email: {ex.Message}";
            }

            return View();
        }
    }
}