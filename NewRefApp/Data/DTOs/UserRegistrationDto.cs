﻿using NewRefApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewRefApp.Data.DTOs
{
    public class UserRegistrationDto
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string WithdrawnPassword { get; set; }

        [Display(Name = "Referral Code")]
        public string ReferralCode { get; set; }
    }

    public class ReferralDto
    {
        public LevelData Level1 { get; set; }
        public LevelData Level2 { get; set; }
        public LevelData Level3 { get; set; }
    }

    public class LevelData
    {
        public int TotalInvites { get; set; }
        public int ActiveInvites { get; set; }
        public decimal TotalInvestmentAmount { get; set; }
        public int Percentage { get; set; }
        public decimal Rebate { get; set; }

        public List<ReferralBonusEntry> BonusEntries { get; set; } = new();
    }

    public class ReferralBonusEntry
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class TeamMemberDataDto
    {
        public List<TeamMemberDetail> Level1 { get; set; } = new List<TeamMemberDetail>();
        public List<TeamMemberDetail> Level2 { get; set; } = new List<TeamMemberDetail>();
        public List<TeamMemberDetail> Level3 { get; set; } = new List<TeamMemberDetail>();
    }

    public class TeamMemberDetail
    {
        public string MobileNumber { get; set; }
        public bool IsPurchased { get; set; }
        public int VipLevel { get; set; }
    }
    public class InvestmentViewModel
    {
        public UserInvestment Investment { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal CalculatedAmount { get; set; }
    }
    public class LoginRequestDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
