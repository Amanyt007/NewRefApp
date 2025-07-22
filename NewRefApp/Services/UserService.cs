using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IReferralProgramService _referralProgramService;

        public UserService(ApplicationDbContext context, IReferralProgramService referralProgramService)
        {
            _context = context;
            _referralProgramService = referralProgramService;
        }

        public async Task<User> RegisterAsync(UserRegistrationDto registrationDto, bool isadmin = false)
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

                user.ReferralCode = await GenerateReferralCode(registrationDto);
                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;

                user.IsAdmin = isadmin;

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
                Level1 = new LevelData(),
                Level2 = new LevelData(),
                Level3 = new LevelData()
            };

            // Fetch rebate percentages from ReferralProgram
            var level1Program = await _referralProgramService.GetReferralProgramByLevelAsync(1);
            var level2Program = await _referralProgramService.GetReferralProgramByLevelAsync(2);
            var level3Program = await _referralProgramService.GetReferralProgramByLevelAsync(3);

            referralData.Level1.Percentage = (int)(level1Program?.Percent ?? 35); // Default to 35 if not found
            referralData.Level2.Percentage = (int)(level2Program?.Percent ?? 2);  // Default to 2 if not found
            referralData.Level3.Percentage = (int)(level3Program?.Percent ?? 1);  // Default to 1 if not found

            // Level 1: Direct referrals
            var level1Users = await _context.Users
                .Where(u => u.ReferredBy == userId)
                .ToListAsync();
            referralData.Level1.TotalInvites = level1Users.Count;
            referralData.Level1.ActiveInvites = level1Users.Count(u => u.IsPurchased);

            // Calculate Level 1 investment and rebate amount
            var level1Investments = await _context.UserInvestment
                .Where(ui => level1Users.Select(u => u.Id).Contains(ui.UserId) && ui.status == 1)
                .Include(ui => ui.InvestmentPlan)
                .ToListAsync();
            referralData.Level1.TotalInvestmentAmount = level1Investments.Sum(ui => ui.InvestmentPlan.InvestmentAmount * ui.PurchaseQuantity);
            referralData.Level1.Rebate = (int)(float)((referralData.Level1.TotalInvestmentAmount * (referralData.Level1.Percentage)) / 100); // Rebate amount in currency

            // Level 2: Referrals from Level 1
            foreach (var level1User in level1Users)
            {
                var level2Users = await _context.Users
                    .Where(u => u.ReferredBy == level1User.Id)
                    .ToListAsync();
                referralData.Level2.TotalInvites += level2Users.Count;
                referralData.Level2.ActiveInvites += level2Users.Count(u => u.IsPurchased);

                // Calculate Level 2 investment and rebate amount
                var level2Investments = await _context.UserInvestment
                    .Where(ui => level2Users.Select(u => u.Id).Contains(ui.UserId) && ui.status == 1)
                    .Include(ui => ui.InvestmentPlan)
                    .ToListAsync();
                var level2InvestmentAmount = level2Investments.Sum(ui => ui.InvestmentPlan.InvestmentAmount * ui.PurchaseQuantity);
                referralData.Level2.TotalInvestmentAmount += level2InvestmentAmount;
                referralData.Level2.Rebate += (int)(float)((level2InvestmentAmount * (referralData.Level2.Percentage)) / 100); // Accumulate rebate amount
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

                    // Calculate Level 3 investment and rebate amount
                    var level3Investments = await _context.UserInvestment
                        .Where(ui => level3Users.Select(u => u.Id).Contains(ui.UserId) && ui.status == 1)
                        .Include(ui => ui.InvestmentPlan)
                        .ToListAsync();
                    var level3InvestmentAmount = level3Investments.Sum(ui => ui.InvestmentPlan.InvestmentAmount * ui.PurchaseQuantity);
                    referralData.Level3.TotalInvestmentAmount += level3InvestmentAmount;
                    referralData.Level3.Rebate += (int)(float)((level3InvestmentAmount * (referralData.Level3.Percentage)) / 100); // Accumulate rebate amount
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
        public async Task<User> UpdateToPurchased(string phoneNumber)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var userInvestmentPlans = await _context.UserInvestment
                .Where(ui => ui.UserId == user.Id && ui.status == 1)
                .Include(ui => ui.InvestmentPlan)
                .ToListAsync();

            // Filter only valid investments where InvestmentPlan is not null
            var validInvestments = userInvestmentPlans
                .Where(ui => ui.InvestmentPlan != null)
                .ToList();

            if (validInvestments.Any())
            {
                user.VipLevel = (int)validInvestments
                    .Max(ui => ui.InvestmentPlan.VipLevel);
            }
            else
            {
                user.VipLevel = 1; // Default to level 1 if no valid investments
            }

            user.IsPurchased = true;
            await _context.SaveChangesAsync();

            return user;
        }

    }
}