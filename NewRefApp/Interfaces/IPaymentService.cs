using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IPaymentService
    {
        Task<UpiDetails> AddUpiDetails(UpiDetails upiDetails);
        Task<BankDetails> AddBankDetails(BankDetails bankDetails);
        Task<IEnumerable<UpiDetails>> GetUserUpiDetails(int userId);
        Task<IEnumerable<BankDetails>> GetUserBankDetails(int userId);
        Task<bool> RemoveUpiDetails(int upiId);
        Task<bool> RemoveBankDetails(int bankId);
    }

}
