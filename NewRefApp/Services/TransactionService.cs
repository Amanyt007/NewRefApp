using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;

namespace NewRefApp.Services
{
    // TransactionService.cs
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IDepositService _depositService;
        private readonly IReferralProgramService _referralProgramService;

        public TransactionService(ApplicationDbContext context, IDepositService depositService, IReferralProgramService referralProgramService, IUserService userService)
        {
            _context = context;
            _depositService = depositService;
            _referralProgramService = referralProgramService;
            _userService = userService;
        }

        public async Task<UserTransactionsViewModel> GetUserTransactionsAsync(int userId)
        {
            var viewModel = new UserTransactionsViewModel
            {
                Transactions = new List<TransactionViewModel>(),
                UserBalance = await CalculateUserBalanceAsync(userId)
            };

            // 1. Deposits
            var deposits = await _context.Deposit
                .Where(d => d.UserId == userId && d.Status == 1)
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            foreach (var deposit in deposits)
            {
                viewModel.Transactions.Add(new TransactionViewModel
                {
                    TransactionType = "Deposit",
                    Amount = deposit.Amount,
                    TransactionDate = deposit.Date,
                    DetailsLink = "+"
                });
            }

            // 2. Withdrawals
            var withdrawals = await _context.Withdraw
                .Where(w => w.UserId == userId && w.Status == 1)
                .OrderByDescending(w => w.Date)
                .ToListAsync();

            foreach (var withdraw in withdrawals)
            {
                viewModel.Transactions.Add(new TransactionViewModel
                {
                    TransactionType = "Withdraw",
                    Amount = withdraw.Amount,
                    TransactionDate = withdraw.Date,
                    DetailsLink = "-"
                });
            }

            // 3. Investment Earnings (completed only)
            var completedInvestments = await _context.UserInvestment
                .Include(ui => ui.InvestmentPlan)
                .Where(ui => ui.UserId == userId)
                .ToListAsync();

            foreach (var investment in completedInvestments)
            {
                var earning = investment.InvestmentPlan.InvestmentAmount * investment.PurchaseQuantity;

                viewModel.Transactions.Add(new TransactionViewModel
                {
                    TransactionType = "Investment Amount",
                    Amount = earning,
                    TransactionDate = investment.StartDate,
                    DetailsLink = "-"
                });
            }

            //foreach (var investment in completedInvestments)
            //{
            //    var fullDays = (int)((DateTime.UtcNow - investment.StartDate).TotalHours / 24);
            //    //var fullDays = investment.StartDate;
            //    var totalDays = int.Parse(investment.InvestmentPlan.RevenueDurationValue);
            //    for (int i = 0; i < totalDays; i++)
            //    {
            //        var dailyEarnings = investment.InvestmentPlan.DailyEarningsPerUnit ?? 0;
            //        var earningAmount = dailyEarnings * investment.PurchaseQuantity;
            //        var lastEarningDate = investment.StartDate.AddDays(i);

            //        viewModel.Transactions.Add(new TransactionViewModel
            //        {
            //            TransactionType = "Daily Earning",
            //            Amount = earningAmount,
            //            TransactionDate = lastEarningDate,
            //            DetailsLink = "+"
            //        });
            //    }
            //}

            foreach (var investment in completedInvestments)
            {
                var endDate = investment.StartDate.AddDays(int.Parse(investment.InvestmentPlan.RevenueDurationValue));
                var timeSpan = (investment.StartDate.AddDays(int.Parse(investment.InvestmentPlan.RevenueDurationValue)) - investment.StartDate).Days;
                var timeSpanFromStartDate = DateTime.UtcNow - investment.StartDate;

                // If the StartDate is in the future, then no days have passed yet
                int daysPassed = (timeSpanFromStartDate.TotalDays < 0) ? 0 : (int)timeSpanFromStartDate.TotalDays;

                //var timeSpan = investment.InvestmentPlan.RevenueDurationValue - investment.StartDate;
                //var fullDays = timeSpan.Days; // Number of complete days elapsed
                //var totalDays = int.Parse(investment.InvestmentPlan.RevenueDurationValue);

                for (int i = 0; i < daysPassed; i++) // Limit to totalDays
                {
                    var dailyEarnings = investment.InvestmentPlan.DailyEarningsPerUnit ?? 0;
                    var earningAmount = dailyEarnings * investment.PurchaseQuantity;
                    var lastEarningDate = investment.StartDate.AddDays(i+1);

                    viewModel.Transactions.Add(new TransactionViewModel
                    {
                        TransactionType = "Daily Earning",
                        Amount = earningAmount,
                        TransactionDate = lastEarningDate,
                        DetailsLink = "+"
                    });
                }
            }


            // 4. Referral Bonuses (Level 1, 2, 3)
            var referralData = await GetReferralDataAsync(userId);
            if (referralData != null)
            {
                foreach (var bonus in referralData.Level1.BonusEntries)
                {
                    viewModel.Transactions.Add(new TransactionViewModel
                    {
                        TransactionType = "Referral Bonus (Level 1)",
                        Amount = bonus.Amount,
                        TransactionDate = bonus.Date,
                        DetailsLink = "+"
                    });
                }

                foreach (var bonus in referralData.Level2.BonusEntries)
                {
                    viewModel.Transactions.Add(new TransactionViewModel
                    {
                        TransactionType = "Referral Bonus (Level 2)",
                        Amount = bonus.Amount,
                        TransactionDate = bonus.Date,
                        DetailsLink = "+"
                    });
                }

                foreach (var bonus in referralData.Level3.BonusEntries)
                {
                    viewModel.Transactions.Add(new TransactionViewModel
                    {
                        TransactionType = "Referral Bonus (Level 3)",
                        Amount = bonus.Amount,
                        TransactionDate = bonus.Date,
                        DetailsLink = "+"
                    });
                }
            }

            // 5. Sort all transactions by date descending
            viewModel.Transactions = viewModel.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return viewModel;
        }


        public async Task<decimal> CalculateUserBalanceAsync(int userId)
        {
            try
            {
                // Total Deposits
                var totalDeposits = await _context.Deposit
                    .Where(d => d.UserId == userId && d.Status == 1)
                    .SumAsync(d => d.Amount);

                // Total Investments
                var totalInvestments = await _context.UserInvestment
                    .Where(ui => ui.UserId == userId && ui.status == 1)
                    .SumAsync(ui => ui.InvestmentPlan.InvestmentAmount * ui.PurchaseQuantity);

                // Total Withdrawals
                var totalWithdrawals = await _context.Withdraw
                    .Where(w => w.UserId == userId && w.Status == 1)
                    .SumAsync(w => w.Amount);

                // Accrued Profit (calculate in memory)
                //var investments = await _context.UserInvestment
                //    .Where(ui => ui.UserId == userId && ui.status == 1)
                //    .Select(ui => new
                //    {
                //        ui.PurchaseQuantity,
                //        ui.StartDate,
                //        DailyEarnings = ui.InvestmentPlan.DailyEarningsPerUnit ?? 0,
                //        duration = int.Parse(ui.InvestmentPlan.RevenueDurationValue) // Assuming this is the duration in days
                //    })
                //    .ToListAsync();

                //decimal totalAccruedProfit = 0;

                //foreach (var inv in investments)
                //{
                //    //var hoursElapsed = (DateTime.UtcNow - inv.StartDate).TotalHours;
                //    //var fullDays = (int)(hoursElapsed / 24);
                //    totalAccruedProfit += inv.DailyEarnings * inv.PurchaseQuantity * inv.duration;
                //}

                var investments = await _context.UserInvestment
                    .Where(ui => ui.UserId == userId && ui.status == 1)
                    .Select(ui => new
                    {
                        ui.PurchaseQuantity,
                        ui.StartDate,
                        DailyEarnings = ui.InvestmentPlan.DailyEarningsPerUnit ?? 0,
                        Duration = int.Parse(ui.InvestmentPlan.RevenueDurationValue)
                    }).ToListAsync();

                decimal totalAccruedProfit = 0;

                foreach (var inv in investments)
                {
                    var timeSpan = DateTime.UtcNow - inv.StartDate;
                    var fullDays = timeSpan.Days;
                    var accruedDays = Math.Min(fullDays, inv.Duration);

                    totalAccruedProfit += inv.DailyEarnings * inv.PurchaseQuantity * accruedDays;
                }
                // Fetch referral data to calculate rebate amounts
                var referralData = await GetReferralDataAsync(userId);
                decimal totalReferralRebate = 0;
                if (referralData != null)
                {
                    totalReferralRebate = (referralData.Level1?.Rebate ?? 0) +
                                         (referralData.Level2?.Rebate ?? 0) +
                                         (referralData.Level3?.Rebate ?? 0);
                }

                // Final Balance
                var grossBalance = totalDeposits - totalInvestments - totalWithdrawals + totalAccruedProfit + totalReferralRebate;
                return grossBalance;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //public async Task<BalanceDetailsDto> GetUserDetails(string phoneNumber)
        //{
        //    try
        //    {
        //        var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        //        var totalAmount = await CalculateUserBalanceAsync(user.Id);
        //        var totalEarning = await CalculateUserEarningAsync(user.Id);
        //        var totalRecharge = await _context.Deposit
        //            .Where(d => d.UserId == user.Id)
        //            .SumAsync(d => d.Amount);
        //        BalanceDetailsDto balanceDetailsDto = new BalanceDetailsDto
        //        {
        //            userId = user.Id,
        //            vipLevel = user.VipLevel,
        //            totalBalance = totalAmount,
        //            earningBalance = totalEarning,
        //            rechargeBalance = totalRecharge
        //        };
        //        return balanceDetailsDto;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}
        public async Task<BalanceDetailsDto> GetUserDetails(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                    throw new ArgumentException("Phone number is required");

                var user = await _context.Users.Where(u => u.PhoneNumber == phoneNumber)
                    .FirstOrDefaultAsync();

                if (user == null)
                    throw new InvalidOperationException("User not found");

                var totalAmount = await CalculateUserBalanceAsync(user.Id);
                var totalEarning = await CalculateUserEarningAsync(user.Id);

                var totalRecharge = await _context.Deposit
                    .Where(d => d.UserId == user.Id && d.Status == 1)
                    .SumAsync(d => (decimal?)d.Amount) ?? 0;

                return new BalanceDetailsDto
                {
                    userId = user.Id,
                    vipLevel = user.VipLevel,
                    totalBalance = totalAmount,
                    earningBalance = totalEarning,
                    rechargeBalance = totalRecharge
                };
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<decimal> CalculateUserEarningAsync(int userId)
        {
            try
            {

                // Accrued Profit (calculate in memory)
                var investments = await _context.UserInvestment
                    .Where(ui => ui.UserId == userId && ui.status == 1)
                    .Select(ui => new
                    {
                        ui.PurchaseQuantity,
                        ui.StartDate,
                        DailyEarnings = ui.InvestmentPlan.DailyEarningsPerUnit ?? 0,
                        duration = int.Parse(ui.InvestmentPlan.RevenueDurationValue) // Assuming this is the duration in days
                    })
                    .ToListAsync();

                decimal totalAccruedProfit = 0;

                foreach (var inv in investments)
                {
                    var timeSpanFromStartDate = DateTime.UtcNow - inv.StartDate;

                    // If the StartDate is in the future, then no days have passed yet
                    //int daysPassed = (timeSpanFromStartDate.TotalDays < 0) ? 0 : (int)timeSpanFromStartDate.TotalDays;
                    //var hoursElapsed = (DateTime.UtcNow - inv.StartDate).TotalHours;
                    //var fullDays = (int)(hoursElapsed / 24);

                    var timeSpan = DateTime.UtcNow - inv.StartDate;
                    var fullDays = timeSpan.Days;
                    var accruedDays = Math.Min(fullDays, inv.duration);

                    //totalAccruedProfit += inv.DailyEarnings * inv.PurchaseQuantity * accruedDays;

                    totalAccruedProfit += inv.DailyEarnings * inv.PurchaseQuantity * accruedDays;
                }

                // Fetch referral data to calculate rebate amounts
                var referralData = await GetReferralDataAsync(userId);
                decimal totalReferralRebate = 0;
                if (referralData != null)
                {
                    totalReferralRebate = (referralData.Level1?.Rebate ?? 0) +
                                         (referralData.Level2?.Rebate ?? 0) +
                                         (referralData.Level3?.Rebate ?? 0);
                }

                // Final Balance
                var grossBalance = totalAccruedProfit + totalReferralRebate;
                return grossBalance;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<decimal> ReferralAmuntAsync(int userId)
        {
            var referralData = await GetReferralDataAsync(userId);
            decimal totalReferralRebate = 0;
            if (referralData != null)
            {
                totalReferralRebate = (referralData.Level1?.Rebate ?? 0) +
                                     (referralData.Level2?.Rebate ?? 0) +
                                     (referralData.Level3?.Rebate ?? 0);
            }
            return totalReferralRebate;
        }

        public async Task<ReferralDto> GetReferralDataAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            var referralData = new ReferralDto
            {
                Level1 = new LevelData(),
                Level2 = new LevelData(),
                Level3 = new LevelData()
            };

            // Fetch rebate percentages from ReferralProgram
            var level1Program = await _referralProgramService.GetReferralProgramByLevelAsync(1);
            var level2Program = await _referralProgramService.GetReferralProgramByLevelAsync(2);
            var level3Program = await _referralProgramService.GetReferralProgramByLevelAsync(3);

            referralData.Level1.Percentage = (int)(level1Program?.Percent ?? 35);
            referralData.Level2.Percentage = (int)(level2Program?.Percent ?? 2);
            referralData.Level3.Percentage = (int)(level3Program?.Percent ?? 1);

            // Level 1
            var level1Users = await _context.Users.Where(u => u.ReferredBy == userId).ToListAsync();
            referralData.Level1.TotalInvites = level1Users.Count;
            referralData.Level1.ActiveInvites = level1Users.Count(u => u.IsPurchased);

            var level1Investments = await _context.UserInvestment
                .Where(ui => level1Users.Select(u => u.Id).Contains(ui.UserId) && ui.status == 1)
                .Include(ui => ui.InvestmentPlan)
                .ToListAsync();

            foreach (var inv in level1Investments)
            {
                decimal totalAmount = inv.InvestmentPlan.InvestmentAmount * inv.PurchaseQuantity;
                referralData.Level1.TotalInvestmentAmount += totalAmount;

                int durationDays = (inv.EndDate - inv.StartDate).Days;
                if (durationDays <= 0) durationDays = 1; // fallback

                decimal totalRebate = (totalAmount * referralData.Level1.Percentage) / 100;
                referralData.Level1.Rebate += totalRebate;

                decimal dailyRebate = totalRebate;

                for (int i = 0; i < durationDays; i++)
                {
                    referralData.Level1.BonusEntries.Add(new ReferralBonusEntry
                    {
                        Amount = Math.Round(dailyRebate, 2),
                        Date = inv.StartDate.AddDays(i)
                    });
                    break;
                }
            }

            // Level 2
            foreach (var level1User in level1Users)
            {
                var level2Users = await _context.Users.Where(u => u.ReferredBy == level1User.Id).ToListAsync();
                referralData.Level2.TotalInvites += level2Users.Count;
                referralData.Level2.ActiveInvites += level2Users.Count(u => u.IsPurchased);

                var level2Investments = await _context.UserInvestment
                    .Where(ui => level2Users.Select(u => u.Id).Contains(ui.UserId) && ui.status == 1)
                    .Include(ui => ui.InvestmentPlan)
                    .ToListAsync();

                foreach (var inv in level2Investments)
                {
                    decimal totalAmount = inv.InvestmentPlan.InvestmentAmount * inv.PurchaseQuantity;
                    referralData.Level2.TotalInvestmentAmount += totalAmount;

                    int durationDays = (inv.EndDate - inv.StartDate).Days;
                    if (durationDays <= 0) durationDays = 1;

                    decimal totalRebate = (totalAmount * referralData.Level2.Percentage) / 100;
                    referralData.Level2.Rebate += totalRebate;

                    decimal dailyRebate = totalRebate;

                    for (int i = 0; i < durationDays; i++)
                    {
                        referralData.Level2.BonusEntries.Add(new ReferralBonusEntry
                        {
                            Amount = Math.Round(dailyRebate, 2),
                            Date = inv.StartDate.AddDays(i)
                        });
                        break;
                    }
                    
                }
            }

            // Level 3
            foreach (var level1User in level1Users)
            {
                var level2Users = await _context.Users.Where(u => u.ReferredBy == level1User.Id).ToListAsync();
                foreach (var level2User in level2Users)
                {
                    var level3Users = await _context.Users.Where(u => u.ReferredBy == level2User.Id).ToListAsync();
                    referralData.Level3.TotalInvites += level3Users.Count;
                    referralData.Level3.ActiveInvites += level3Users.Count(u => u.IsPurchased);

                    var level3Investments = await _context.UserInvestment
                        .Where(ui => level3Users.Select(u => u.Id).Contains(ui.UserId) && ui.status == 1)
                        .Include(ui => ui.InvestmentPlan)
                        .ToListAsync();

                    foreach (var inv in level3Investments)
                    {
                        decimal totalAmount = inv.InvestmentPlan.InvestmentAmount * inv.PurchaseQuantity;
                        referralData.Level3.TotalInvestmentAmount += totalAmount;

                        int durationDays = (inv.EndDate - inv.StartDate).Days;
                        if (durationDays <= 0) durationDays = 1;

                        decimal totalRebate = (totalAmount * referralData.Level3.Percentage) / 100;
                        referralData.Level3.Rebate += totalRebate;

                        decimal dailyRebate = totalRebate;

                        for (int i = 0; i < durationDays; i++)
                        {
                            referralData.Level3.BonusEntries.Add(new ReferralBonusEntry
                            {
                                Amount = Math.Round(dailyRebate, 2),
                                Date = inv.StartDate.AddDays(i)
                            });
                            break;
                        }
                        
                    }
                }
            }

            return referralData;
        }

        // Move your GetUserTransactionsAsync code here
    }

}
