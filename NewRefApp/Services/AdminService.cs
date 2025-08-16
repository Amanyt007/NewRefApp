using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ITransactionService _transactionService;

        public AdminService(ApplicationDbContext context, IUserService userService, ITransactionService transactionService)
        {
            _context = context;
            _userService = userService;
            _transactionService = transactionService;
        }

        public async Task<User?> GetLoggedInUserAsync(string userPhone)
        {
            return await _userService.GetByPhoneAsync(userPhone);
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            return await _transactionService.CalculateUserBalanceAsync(userId);
        }

        public async Task<List<Deposit>> GetPendingDepositsAsync()
        {
            return await _context.Deposit.Where(d => d.Status == 0).ToListAsync();
        }

        //public async Task<List<Withdraw>> GetPendingWithdrawsAsync()
        //{
        //    return await _context.Withdraw.Where(w => w.Status == 0).ToListAsync();
        //}
        public async Task<List<WithdrawDto>> GetPendingWithdrawsAsync(int? statusFilter = null, string phoneSearch = null)
        {
            var query = _context.Withdraw
                .Include(w => w.User)
                .Include(w => w.BankDetail)
                .Include(w => w.UpiDetail)
                .AsQueryable();

            if (statusFilter.HasValue)
            {
                query = query.Where(w => w.Status == statusFilter.Value);
            }

            if (!string.IsNullOrEmpty(phoneSearch))
            {
                query = query.Where(w => w.User.PhoneNumber.Contains(phoneSearch));
            }

            return await query
                .Select(w => new WithdrawDto
                {
                    Id = w.Id,
                    UserName = w.User.FullName,
                    PhoneNumber = w.User.PhoneNumber,
                    Amount = w.Amount,
                    Status = w.Status,
                    PaymentType = w.PaymentType,
                    UpiId = w.PaymentType == 0 && w.UpiDetail != null ? w.UpiDetail.UpiId : "N/A",
                    BankAccount = w.PaymentType == 1 && w.BankDetail != null
                                  ? $"{w.BankDetail.AccountNumber} ({w.BankDetail.BankName})"
                                  : "N/A",
                    Date = w.Date
                })
                .ToListAsync();
        }



        public async Task<Withdraw?> GetWithdrawByIdAsync(int id)
        {
            return await _context.Withdraw.FindAsync(id);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.OrderByDescending(x=>x.CreatedAt).ToListAsync();
        }

        public async Task<List<User>> GetFilteredUsersAsync(string filter)
        {
            var query = _context.Users.AsQueryable();

            if (filter == "admin")
                query = query.Where(u => u.IsAdmin);
            else if (filter == "purchased")
                query = query.Where(u => u.IsPurchased);
            else if (filter == "user")
                query = query.Where(u => !u.IsAdmin);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

    }

}
