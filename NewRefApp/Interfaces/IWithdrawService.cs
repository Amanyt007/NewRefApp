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
    }
}
