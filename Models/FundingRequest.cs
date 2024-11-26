namespace BumbleBee.Models
{
    public class FundingRequest
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyBackground { get; set; }
        public string ProjectDetails { get; set; }
        public decimal FundingAmount { get; set; }
        public string IntendedImpact { get; set; }
        public string SupportingDocumentPath { get; set; } // Add this property
    }
}
