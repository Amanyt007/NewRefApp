using NewRefApp.Data.DTOs;

namespace NewRefApp.Services
{
    public interface ITransactionService
    {
        Task<UserTransactionsViewModel> GetUserTransactionsAsync(int userId);
        Task<decimal> CalculateUserBalanceAsync(int userId);
        Task<BalanceDetailsDto> GetUserDetails(string phoneNumber);
        Task<decimal> CalculateUserEarningAsync(int userId);
        Task<ReferralDto> GetReferralDataAsync(int userId);
        Task<decimal> ReferralAmuntAsync(int userId);
    }
}
