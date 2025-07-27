using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IAdminService
    {
        Task<User?> GetLoggedInUserAsync(string userPhone);
        Task<decimal> GetUserBalanceAsync(int userId);
        Task<List<Deposit>> GetPendingDepositsAsync();
        Task<Withdraw?> GetWithdrawByIdAsync(int id);
        Task<List<Withdraw>> GetPendingWithdrawsAsync();
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetFilteredUsersAsync(string filter);
        Task<User> GetUserByIdAsync(int id);

    }
}
