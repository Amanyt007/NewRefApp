using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IWithdrawService
    {
        Task<IEnumerable<Withdraw>> GetAllWithdrawsAsync();
        Task<Withdraw> GetWithdrawByIdAsync(int id);
        Task<Withdraw> GetWithdrawByUserIdAsync(int userId);
        Task<IEnumerable<Withdraw>> GetWithdrawalsByUserIdAsync(int userId);
        Task CreateWithdrawAsync(Withdraw withdraw);
        Task UpdateWithdrawAsync(Withdraw withdraw);
        Task DeleteWithdrawAsync(int id);

        // New methods for SettleWithdraw and SuccessfulWithdraws
        Task<IEnumerable<Withdraw>> GetPendingWithdrawsAsync(); // For SettleWithdraw (Status = 0)
        Task<IEnumerable<Withdraw>> GetSuccessfulWithdrawsAsync(); // For SuccessfulWithdraws (Status = 1)
    }
}
