using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;
using QRCoder;



namespace NewRefApp.Services
{
    public class UpiDetailsService : IUpiDetailsService
    {
        private readonly ApplicationDbContext _context;

        public UpiDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UpiDetails> GetUpiDetailByIdAsync(int id)
        {
            return await _context.UpiDetails
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<UpiDetails>> GetAllUpiDetailsAsync()
        {
            return await _context.UpiDetails
                .Include(u => u.User)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
        }

        public async Task CreateUpiDetailAsync(UpiDetails upiDetails)
        {
            _context.UpiDetails.Add(upiDetails);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUpiDetailAsync(UpiDetails upiDetails)
        {
            _context.UpiDetails.Update(upiDetails);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUpiDetailAsync(int id)
        {
            var upiDetails = await _context.UpiDetails.FindAsync(id);
            if (upiDetails != null)
            {
                _context.UpiDetails.Remove(upiDetails);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UpiDetails> GetFirstActiveAdminUpiAsync()
        {
            return await _context.UpiDetails
                .Where(u => u.IsAdmin && u.Status)
                .OrderBy(u => u.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<UpiDetails> GetUpiDetailsByUserIdAsync(int userId)
        {
            return await _context.UpiDetails
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public string GenerateUpiQrCode(string upiId, decimal amount)
        {
            string upiUrl = $"upi://pay?pa={upiId}&pn=YourName&am={amount}&cu=INR&tn=Payment";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(upiUrl, QRCodeGenerator.ECCLevel.Q);
                var pngQrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = pngQrCode.GetGraphic(20); // 20 is pixel-per-module

                return Convert.ToBase64String(qrCodeBytes); // Can be used in <img src="data:image/png;base64,...">
            }
        }


    }
}
