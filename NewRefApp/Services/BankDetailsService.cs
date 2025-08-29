using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class BankDetailsService : IBankDetailsService
    {
        private readonly ApplicationDbContext _context;

        public BankDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BankDetails> GetBankDetailByIdAsync(int id)
        {
            return await _context.BankDetails
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BankDetails>> GetAllBankDetailsAsync()
        {
            return await _context.BankDetails
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<BankDetails>> GetBankDetailsAsync(string roleFilter, string phoneSearch)
        {
            var query = _context.BankDetails
                .Include(b => b.User)
                .AsQueryable();

            // Role filter (admin/user)
            if (!string.IsNullOrEmpty(roleFilter))
            {
                if (roleFilter == "admin")
                    query = query.Where(b => b.IsAdmin);
                else if (roleFilter == "user")
                    query = query.Where(b => !b.IsAdmin);
            }

            // Phone search
            if (!string.IsNullOrEmpty(phoneSearch))
            {
                query = query.Where(b => b.User.PhoneNumber.Contains(phoneSearch));
            }

            return await query.OrderByDescending(b => b.CreatedDate).ToListAsync();
        }


        public async Task CreateBankDetailAsync(BankDetails bankDetails,bool isAdmin = false)
        {
            if (isAdmin)
            {
                bankDetails.IsAdmin = true; // Assuming you want to set IsAdmin to true for admin users
            }
            else
            {
                bankDetails.IsAdmin = false; // Set to false for non-admin users
            }
            _context.BankDetails.Add(bankDetails);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBankDetailAsync(BankDetails bankDetails)
        {
            _context.BankDetails.Update(bankDetails);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBankDetailAsync(int id)
        {
            var bankDetails = await _context.BankDetails.FindAsync(id);
            if (bankDetails != null)
            {
                _context.BankDetails.Remove(bankDetails);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BankDetails> GetRandomActiveAdminBankAsync()
        {
            var bank = await _context.BankDetails
        .Where(b => b.IsAdmin && b.Status)
        .OrderBy(b => b.LastUsedAt ?? DateTime.MinValue)
        .ThenBy(b => b.Id)
        .FirstOrDefaultAsync();

            if (bank != null)
            {
                bank.LastUsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return bank;
        }


        public async Task<BankDetails> GetFirstActiveAdminBankAsync()
        {
            return await _context.BankDetails
                .Where(b => b.IsAdmin && b.Status) // Assuming User has IsAdmin and Status properties
                .OrderBy(b => b.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<BankDetails> GetBankDetailsByUserIdAsync(int userId)
        {
            return await _context.BankDetails
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }
    }
}
