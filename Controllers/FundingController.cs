using BumbleBee.Data;
using BumbleBee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;

namespace BumbleBee.Controllers
{
    public class FundingController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FundingController(DataContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FundingRequestViewModel model, IFormFile SupportingDocument)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (SupportingDocument != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + SupportingDocument.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await SupportingDocument.CopyToAsync(fileStream);
                    }
                }

                var fundingRequest = new FundingRequest
                {
                    CompanyName = model.CompanyName,
                    CompanyBackground = model.CompanyBackground,
                    ProjectDetails = model.ProjectDetails,
                    FundingAmount = model.FundingAmount,
                    IntendedImpact = model.IntendedImpact,
                    SupportingDocumentPath = uniqueFileName != null ? "/uploads/" + uniqueFileName : null
                };

                _context.FundingRequests.Add(fundingRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("Success");
            }

            return View(model);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
