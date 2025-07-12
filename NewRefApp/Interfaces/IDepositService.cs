using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IDepositService
    {
        Task<Deposit> GetDepositByIdAsync(int id);
        Task<IEnumerable<Deposit>> GetAllDepositsAsync();
        Task CreateDepositAsync(Deposit deposit);
        Task UpdateDepositAsync(Deposit deposit);
        Task DeleteDepositAsync(int id);
        Task<User> GetUserByPhoneAsync(string phoneNumber);
    }
}
