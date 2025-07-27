using NewRefApp.Data.DTOs;

namespace NewRefApp.Services
{
    public interface ITransactionService
    {
        Task<UserTransactionsViewModel> GetUserTransactionsAsync(int userId);
        Task<decimal> CalculateUserBalanceAsync(int userId);
    }
}
