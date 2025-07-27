using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;
using NewRefApp.Models;
using System.Security.Policy;

namespace NewRefApp.Services
{
    public class DepositService : IDepositService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public DepositService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<Deposit> GetDepositByIdAsync(int id)
        {
            return await _context.Deposit
                .Include(d => d.User)
                .Include(d => d.BankDetail)
                .Include(d => d.UpiDetail)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Deposit>> GetAllDepositsAsync()
        {
            return await _context.Deposit
                .Include(d => d.User)
                .Include(d => d.BankDetail)
                .Include(d => d.UpiDetail)
                .ToListAsync();
        }

        public async Task CreateDepositAsync(Deposit deposit)
        {
            _context.Deposit.Add(deposit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDepositAsync(Deposit deposit)
        {
            _context.Deposit.Update(deposit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepositAsync(int id)
        {
            var deposit = await _context.Deposit.FindAsync(id);
            if (deposit != null)
            {
                _context.Deposit.Remove(deposit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        //public async Task<decimal> CalculateUserBalanceAsync(int userId)
        //{
        //    try
        //    {
        //        // Calculate total deposits (approved only, assuming Status = 1)
        //        var totalDeposits = await _context.Deposit
        //            .Where(d => d.UserId == userId && d.Status == 1)
        //            .SumAsync(d => d.Amount);

        //        // Calculate total investments (purchased only, assuming status = 1)
        //        var totalInvestments = await _context.UserInvestment
        //            .Where(ui => ui.UserId == userId && ui.status == 1)
        //            .SumAsync(ui => ui.InvestmentPlan.InvestmentAmount * ui.PurchaseQuantity);

        //        // Calculate total withdrawals (completed only, assuming Status = 1)
        //        var totalWithdrawals = await _context.Withdraw
        //            .Where(w => w.UserId == userId && w.Status == 1)
        //            .SumAsync(w => w.Amount);

        //        // Balance = Total Deposits - Total Investments - Total Withdrawals
        //        return totalDeposits - totalInvestments - totalWithdrawals;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


    }
}
