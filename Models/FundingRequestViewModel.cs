namespace BumbleBee.Models
{
    public class FundingRequestViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyBackground { get; set; }
        public string ProjectDetails { get; set; }
        public decimal FundingAmount { get; set; }
        public string IntendedImpact { get; set; }
        public IFormFile SupportingDocument { get; set; }
    }
}
