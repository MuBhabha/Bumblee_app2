using PayPal.Api;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using MimeKit;
using System;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace BumbleBee.Utility
{
	public class PayPalService
	{
		private readonly IConfiguration _configuration;

		public PayPalService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public APIContext GetAPIContext()
		{
			var clientId = _configuration["PayPal:ClientId"];
			var clientSecret = _configuration["PayPal:ClientSecret"];
			var config = new Dictionary<string, string>
			{
				{ "mode", _configuration["PayPal:Mode"] }
			};

			var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
			return new APIContext(accessToken);
		}

		public Payment CreatePayment(decimal amount, string returnUrl, string cancelUrl)
		{
			var apiContext = GetAPIContext();

			var payment = new Payment
			{
				intent = "sale",
				payer = new Payer { payment_method = "paypal" },
				transactions = new List<Transaction>
				{
					new Transaction
					{
						description = "Transaction description",
						invoice_number = Guid.NewGuid().ToString(), // Unique invoice number
                        amount = new Amount
						{
							currency = "USD",
							total = amount.ToString("F2") // Total amount
                        }
					}
				},
				redirect_urls = new RedirectUrls
				{
					cancel_url = cancelUrl,
					return_url = returnUrl
				}
			};

			return payment.Create(apiContext);
		}

		public async Task<Payment> GetPaymentDetailsAsync(string paymentId)
		{
			var apiContext = GetAPIContext();
			return Payment.Get(apiContext, paymentId);
		}

		public async Task SendInvoiceEmailAsync(string paymentId, string recipientEmail)
		{
			try
			{

			var payment = await GetPaymentDetailsAsync(paymentId);
			var invoiceDetails = ExtractInvoiceDetails(payment);
				var password = _configuration["Smtp:Password"].ToString();
				var host = _configuration["Smtp:Host"].ToString();
				var s_email = _configuration["Smtp:Email"].ToString();
				var port = _configuration["Smtp:Port"].ToString();
            // Generate PDF (you can use a library like iTextSharp or any other PDF generation library)
            byte[] pdfBytes = GenerateInvoicePdf(invoiceDetails);

			// Send email with the invoice attached
			var email = new MimeMessage();
			email.From.Add(new MailboxAddress("Bumblebee Foundation", _configuration["Smtp:Email"]));
			email.To.Add(MailboxAddress.Parse(recipientEmail));
			email.Subject = $"Invoice for Payment {paymentId}";

			var builder = new BodyBuilder
			{
				HtmlBody = $"<h1>Invoice for Donation {paymentId}</h1><p>Thank you for your donation!</p>"
			};

			// Attach the PDF
			builder.Attachments.Add($"Invoice-{paymentId}.pdf", pdfBytes, ContentType.Parse("application/pdf"));
			email.Body = builder.ToMessageBody();

			using (var smtpClient = new SmtpClient())
			{
				smtpClient.Connect(host, int.Parse(port), SecureSocketOptions.StartTls);
				smtpClient.Authenticate(s_email,password);
				await smtpClient.SendAsync(email);
				smtpClient.Disconnect(true);
			}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private InvoiceDetails ExtractInvoiceDetails(Payment payment)
		{
			// Extract relevant details from the payment object
			return new InvoiceDetails
			{
				PaymentId = payment.id,
				Amount = decimal.Parse(payment.transactions[0].amount.total),
				Currency = payment.transactions[0].amount.currency,
				PayerEmail = payment.payer.payer_info.email,
				PayerName = $"{payment.payer.payer_info.first_name} {payment.payer.payer_info.last_name}",
				InvoiceNumber = payment.transactions[0].invoice_number
			};
		}

		private byte[] GenerateInvoicePdf(InvoiceDetails invoiceDetails)
		{
			// Implement PDF generation logic here
			// For example, using iTextSharp or any other PDF library
			using (var ms = new MemoryStream())
			{
				//Create PDF document and write to MemoryStream
				//Example using iTextSharp
				Document document = new Document();
				PdfWriter.GetInstance(document, ms);
				document.Open();
				document.Add(new Paragraph($"Invoice: {invoiceDetails.InvoiceNumber}"));
				document.Add(new Paragraph($"Payment ID: {invoiceDetails.PaymentId}"));
				document.Add(new Paragraph($"Currency: {invoiceDetails.Currency}"));
				document.Add(new Paragraph($"Amount: {invoiceDetails.Amount}"));
				document.Add(new Paragraph($"Payer Name: {invoiceDetails.PayerName}"));
				document.Add(new Paragraph($"Payer Email: {invoiceDetails.PayerEmail}"));
				document.Close();

				return ms.ToArray();
			}
		}
		public async Task<byte[]> DownloadInvoice(string paymentId)
		{
			var payment = await GetPaymentDetailsAsync(paymentId);
			var invoiceDetails = ExtractInvoiceDetails(payment);

			// Generate PDF (you can use a library like iTextSharp or any other PDF generation library)
			byte[] pdfBytes = GenerateInvoicePdf(invoiceDetails);

			return pdfBytes;
		}
	}

	public class InvoiceDetails
	{
		public string PaymentId { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; }
		public string PayerEmail { get; set; }
		public string PayerName { get; set; }
		public string InvoiceNumber { get; set; }
	}
}