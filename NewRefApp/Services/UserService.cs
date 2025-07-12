using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Data.DTOs;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                // Check if phone number already exists
                if (await _context.Users.AnyAsync(u => u.PhoneNumber == registrationDto.PhoneNumber))
                {
                    throw new Exception("Phone number already registered");
                }

                var user = new User
                {
                    FullName = registrationDto.FullName,
                    PhoneNumber = registrationDto.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Hash password
                user.Password = registrationDto.Password;
                user.WithdrawnPassword = registrationDto.WithdrawnPassword;
                user.VipLevel = 0;
                user.IsPurchased = false;
                user.CreatedAt = DateTime.UtcNow;
                // Generate referral code if referred
                if (!string.IsNullOrEmpty(registrationDto.ReferralCode))
                {
                    var referrer = await _context.Users.FirstOrDefaultAsync(u => u.ReferralCode == registrationDto.ReferralCode);
                    if (referrer != null)
                    {
                        user.ReferredBy = referrer.Id;
                    }
                    else
                    {
                        user.ReferredBy = 0;
                    }
                }

                user.ReferralCode =  await GenerateReferralCode(registrationDto);
                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;
                user.IsAdmin = false;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<User> LoginAsync(string phoneNumber, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.Password == password);

            if (user == null)
            {
                throw new Exception("Invalid mobile number or password");
            }

            return user;
        }
        public async Task<User> GetByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<string> GenerateReferralCode(UserRegistrationDto registrationDto)
        {
            if (registrationDto == null)
            {
                throw new ArgumentNullException(nameof(registrationDto), "User cannot be null when generating referral code.");
            }

            string baseCode = GenerateNamePart(registrationDto.FullName);
            string referralCode = "";
            bool isUnique = false;
            int currentSuffix = 2001; // Start from 2001

            while (!isUnique)
            {
                referralCode = $"{baseCode}{currentSuffix:D4}";

                // Check if this generated code already exists in the database
                // This is crucial for uniqueness when generating before saving
                bool codeExists = await _context.Users
                    .AnyAsync(u => u.ReferralCode == referralCode);

                if (!codeExists)
                {
                    isUnique = true;
                }
                else
                {
                    currentSuffix++; // Increment and try the next suffix if not unique
                                     // Add a safeguard for extremely rare cases or potential infinite loops if suffixes run out
                    if (currentSuffix > 9999) // If it exceeds 9999, consider alternative logic or throw an error
                    {
                        // Handle this edge case, perhaps by changing baseCode or logging an error.
                        // For simplicity, we'll just break or throw here.
                        throw new InvalidOperationException("Could not generate unique referral code, suffixes exhausted.");
                    }
                }
            }
            return referralCode;
        }
        private string GenerateNamePart(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "XX"; // Default if name is empty or null
            }

            string[] nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length >= 2)
            {
                // First character of the first word, first character of the second word
                return $"{nameParts[0][0]}{nameParts[1][0]}".ToUpper();
            }
            else if (nameParts.Length == 1)
            {
                // First two characters of the single word
                return nameParts[0].Substring(0, Math.Min(2, nameParts[0].Length)).ToUpper();
            }
            else
            {
                return "XX"; // Fallback for very unusual names
            }
        }

        public async Task<ReferralDto> GetReferralDataAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            var referralData = new ReferralDto
            {
                Level1 = new LevelData { Rebate = 35, TotalInvites = 0, ActiveInvites = 0 },
                Level2 = new LevelData { Rebate = 2, TotalInvites = 0, ActiveInvites = 0 },
                Level3 = new LevelData { Rebate = 1, TotalInvites = 0, ActiveInvites = 0 }
            };

            // Level 1: Direct referrals
            var level1Users = await _context.Users
                .Where(u => u.ReferredBy == userId)
                .ToListAsync();
            referralData.Level1.TotalInvites = level1Users.Count;
            referralData.Level1.ActiveInvites = level1Users.Count(u => u.IsPurchased);

            // Level 2: Referrals from Level 1
            foreach (var level1User in level1Users)
            {
                var level2Users = await _context.Users
                    .Where(u => u.ReferredBy == level1User.Id)
                    .ToListAsync();
                referralData.Level2.TotalInvites += level2Users.Count;
                referralData.Level2.ActiveInvites += level2Users.Count(u => u.IsPurchased);
            }

            // Level 3: Referrals from Level 2
            foreach (var level1User in level1Users)
            {
                var level2Users = await _context.Users
                    .Where(u => u.ReferredBy == level1User.Id)
                    .ToListAsync();
                foreach (var level2User in level2Users)
                {
                    var level3Users = await _context.Users
                        .Where(u => u.ReferredBy == level2User.Id)
                        .ToListAsync();
                    referralData.Level3.TotalInvites += level3Users.Count;
                    referralData.Level3.ActiveInvites += level3Users.Count(u => u.IsPurchased);
                }
            }

            return referralData;
        }

        public async Task<TeamMemberDataDto> GetAllTeamMembersAsync(int userId)
        {
            var teamData = new TeamMemberDataDto();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return teamData;

            // Level 1
            teamData.Level1 = await _context.Users
                .Where(u => u.ReferredBy == userId)
                .Select(u => new TeamMemberDetail
                {
                    MobileNumber = u.PhoneNumber,
                    IsPurchased = u.IsPurchased,
                    VipLevel = u.VipLevel
                }).ToListAsync();

            // Level 2
            var level1Users = await _context.Users
                .Where(u => u.ReferredBy == userId)
                .ToListAsync();
            foreach (var level1User in level1Users)
            {
                var level2Users = await _context.Users
                    .Where(u => u.ReferredBy == level1User.Id)
                    .Select(u => new TeamMemberDetail
                    {
                        MobileNumber = u.PhoneNumber,
                        IsPurchased = u.IsPurchased,
                        VipLevel = u.VipLevel
                    }).ToListAsync();
                teamData.Level2.AddRange(level2Users);
            }

            // Level 3
            foreach (var level1User in level1Users)
            {
                var level2Users = await _context.Users
                    .Where(u => u.ReferredBy == level1User.Id)
                    .ToListAsync();
                foreach (var level2User in level2Users)
                {
                    var level3Users = await _context.Users
                        .Where(u => u.ReferredBy == level2User.Id)
                        .Select(u => new TeamMemberDetail
                        {
                            MobileNumber = u.PhoneNumber,
                            IsPurchased = u.IsPurchased,
                            VipLevel = u.VipLevel
                        }).ToListAsync();
                    teamData.Level3.AddRange(level3Users);
                }
            }

            return teamData;
        }
    }
}