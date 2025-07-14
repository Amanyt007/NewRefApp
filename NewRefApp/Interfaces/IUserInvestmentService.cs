using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IUserInvestmentService
    {
        Task<IEnumerable<UserInvestment>> GetAllUserInvestmentsAsync();
        Task<IEnumerable<UserInvestment>> GetUserInvestmentsByUserIdAsync(int userId);
        Task<UserInvestment> GetUserInvestmentByIdAsync(int id);
        Task CreateUserInvestmentAsync(UserInvestment userInvestment);
        Task UpdateUserInvestmentAsync(UserInvestment userInvestment);
        Task DeleteUserInvestmentAsync(int id);
    }
}
