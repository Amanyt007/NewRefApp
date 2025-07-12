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
        Task<User> GetUserByPhoneAsync(string userPhone);
        Task<UpiDetails> GetFirstActiveAdminUpiAsync();
        string GenerateUpiQrCode(string upiId, decimal amount);
    }

}
