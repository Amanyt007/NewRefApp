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
