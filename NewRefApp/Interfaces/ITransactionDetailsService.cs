using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface ITransactionDetailsService
    {
        Task<IEnumerable<Deposit>> GetUserTransactionsAsync(string userPhone);
        Task<Deposit> GetTransactionByIdAsync(int id);
    }
}
