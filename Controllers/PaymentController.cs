using BumbleBee.Utility;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;

namespace BumbleBee.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayPalService _payPalService;

        public PaymentController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreatePayment(decimal amount,decimal custom_amount, string? email)
        {
            if (custom_amount>0)
            {
                amount = custom_amount;
            }
            var returnUrl = Url.Action("ExecutePayment", "Payment", null, Request.Scheme);
            var cancelUrl = Url.Action("CancelPayment", "Payment", null, Request.Scheme);

            var payment = _payPalService.CreatePayment(amount, returnUrl, cancelUrl);

            var approvalUrl = payment.links.FirstOrDefault(lnk => lnk.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

            return Redirect(approvalUrl);
        }

        [HttpGet]
        public async Task<IActionResult> ExecutePayment(string paymentId, string token, string PayerID)
        {
            var apiContext = _payPalService.GetAPIContext();
            var paymentExecution = new PaymentExecution { payer_id = PayerID };
            var payment = Payment.Get(apiContext, paymentId);
            var executedPayment = payment.Execute(apiContext, paymentExecution);
            if (User.Identity.IsAuthenticated)
            {
                await _payPalService.SendInvoiceEmailAsync(paymentId, User.Identity.Name);
            }
            ViewBag.payId= paymentId;
            return View("PaymentSuccess", executedPayment);
        }
        public async Task<IActionResult> DownloadInvoice(string paymentId)
        {
            byte[] pdfBytes=await _payPalService.DownloadInvoice(paymentId);
			return File(pdfBytes, "application/pdf", $"Invoice-{paymentId}.pdf");
		}

        [HttpGet]
        public IActionResult CancelPayment()
        {
            return View("PaymentCancelled");
        }
        
    }
}
