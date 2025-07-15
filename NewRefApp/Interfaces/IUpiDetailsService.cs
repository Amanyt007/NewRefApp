using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IUpiDetailsService
    {
        Task<UpiDetails> GetUpiDetailByIdAsync(int id);
        Task<IEnumerable<UpiDetails>> GetAllUpiDetailsAsync();
        Task CreateUpiDetailAsync(UpiDetails upiDetails);
        Task UpdateUpiDetailAsync(UpiDetails upiDetails);
        Task DeleteUpiDetailAsync(int id);
        Task<UpiDetails> GetFirstActiveAdminUpiAsync();
        Task<UpiDetails> GetUpiDetailsByUserIdAsync(int userId);
        string GenerateUpiQrCode(string upiId, decimal amount);
    }

}
