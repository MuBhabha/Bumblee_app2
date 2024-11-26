using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BumbleBee.Models
{
	public class Donation
	{
        [Key]
        public int Id { get; set; }
        public decimal amount { get; set; } = decimal.Zero;
        public string? paymentId { get; set; }
        public DateTime DonateDate { get; set; } = DateTime.Now;
        [ForeignKey(nameof(Id))]
        public int userId { get; set; }
        public virtual ApplicationUser? User { get; set; }

    }
}
