using NewRefApp.Models;

namespace NewRefApp.Interfaces
{
    public interface IReferralProgramService
    {
        Task<IEnumerable<ReferralProgram>> GetAllReferralProgramsAsync();
        Task<ReferralProgram> GetReferralProgramByIdAsync(int id);
        Task<ReferralProgram> GetReferralProgramByLevelAsync(int level);
        Task CreateReferralProgramAsync(ReferralProgram referralProgram);
        Task UpdateReferralProgramAsync(ReferralProgram referralProgram);
        Task DeleteReferralProgramAsync(int id);
    }
}
