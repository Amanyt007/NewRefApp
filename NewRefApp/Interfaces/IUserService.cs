using NewRefApp.Data.DTOs;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(UserRegistrationDto registrationDto);
        Task<User> LoginAsync(string phoneNumber, string password);
        Task<User> GetByPhoneAsync(string phoneNumber);
        Task<string> GenerateReferralCode(UserRegistrationDto registrationDto);
        Task<ReferralDto> GetReferralDataAsync(int userId);
        Task<TeamMemberDataDto> GetAllTeamMembersAsync(int userId);
    }
}