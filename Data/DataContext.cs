using BumbleBee.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BumbleBee.Data
{
	public class DataContext:IdentityDbContext
	{
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<FundingRequest> FundingRequests { get; set; }

    }
}
