using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class ReferralProgramService : IReferralProgramService
    {
        private readonly ApplicationDbContext _context;

        public ReferralProgramService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReferralProgram>> GetAllReferralProgramsAsync()
        {
            return await _context.ReferralProgram.ToListAsync();
        }

        public async Task<ReferralProgram> GetReferralProgramByIdAsync(int id)
        {
            return await _context.ReferralProgram.FindAsync(id);
        }

        public async Task<ReferralProgram> GetReferralProgramByLevelAsync(int level)
        {
            return await _context.ReferralProgram.FirstOrDefaultAsync(rp => rp.Level == level);
        }

        public async Task CreateReferralProgramAsync(ReferralProgram referralProgram)
        {
            _context.ReferralProgram.Add(referralProgram);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReferralProgramAsync(ReferralProgram referralProgram)
        {
            _context.ReferralProgram.Update(referralProgram);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReferralProgramAsync(int id)
        {
            var referralProgram = await _context.ReferralProgram.FindAsync(id);
            if (referralProgram != null)
            {
                _context.ReferralProgram.Remove(referralProgram);
                await _context.SaveChangesAsync();
            }
        }
    }
}
