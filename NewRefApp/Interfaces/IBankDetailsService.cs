using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IBankDetailsService
    {
        Task<BankDetails> GetBankDetailByIdAsync(int id);
        Task<IEnumerable<BankDetails>> GetAllBankDetailsAsync();
        Task CreateBankDetailAsync(BankDetails bankDetails,bool isAdmin);
        Task UpdateBankDetailAsync(BankDetails bankDetails);
        Task DeleteBankDetailAsync(int id);
        Task<BankDetails> GetFirstActiveAdminBankAsync();
        Task<BankDetails> GetBankDetailsByUserIdAsync(int userId);
    }
}
