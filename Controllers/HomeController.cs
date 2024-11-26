using BumbleBee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace BumbleBee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ContactUs(ContactUsVM model)
        {
            if (ModelState.IsValid) {
                bool temp = SendEmail(model);
                if (temp)
                {
                    ViewBag.msg = "Your query has been submitted successfully.";
                    return View();
                }

            } return View(model);
        }
        public IActionResult FAQs()
        {
            return View();
        }

        public bool SendEmail(ContactUsVM model)
        {
            try
            {
                string email = configuration.GetValue<string>("Smtp:Email") ?? string.Empty;
                string to = configuration.GetValue<string>("Smtp:Email") ?? string.Empty;
                string password = configuration.GetValue<string>("Smtp:Password") ?? string.Empty;
                string host = configuration.GetValue<string>("Smtp:Host") ?? string.Empty;
                int port = configuration.GetValue<int>("Smtp:Port");
                bool enableSSL = configuration.GetValue<bool>("Smtp:EnableSSl");
                using (MailMessage mm = new MailMessage(email, to))
                {
                    mm.Subject =model.Subject;
                    mm.Body = $"<h3>Contact Us Form</h3><h5>You have a query.</h5><p><strong>{model.Name}</strong></p><p>{model.Email}</p><p>{model.Phone}</p><p>{model.Subject}</p><p>{model.Message}</p>";
                    mm.BodyEncoding = System.Text.Encoding.UTF8;
                    mm.SubjectEncoding = System.Text.Encoding.Default;
                    mm.IsBodyHtml = true;
                    var smtpClient = new SmtpClient(host)
                    {
                        Port = port, //alternative port number is 8889
                        Credentials = new NetworkCredential(email, password),
                        EnableSsl = enableSSL
                    };

                    smtpClient.Send(mm);
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
