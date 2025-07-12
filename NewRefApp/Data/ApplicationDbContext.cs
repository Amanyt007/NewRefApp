using NewRefApp.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using NewRefApp.Models;


namespace NewRefApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ReferralProgram> ReferralProgram { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        public DbSet<Withdraw> Withdraw { get; set; }
        public DbSet<InvestmentPlan> InvestmentPlan { get; set; }
        public DbSet<UserInvestment> UserInvestment { get; set; }
        public DbSet<Deposit> Deposit { get; set; }
        public DbSet<BankDetails> BankDetails { get; set; }
        public DbSet<UpiDetails> UpiDetails { get; set; }

    }

}
