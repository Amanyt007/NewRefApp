using System.Diagnostics;
using NewRefApp.Models;
using Microsoft.AspNetCore.Mvc;
using NewRefApp.Middlewares;
using NewRefApp.Services;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;
using System.Text.RegularExpressions;

namespace NewRefApp.Controllers
{
    [ServiceFilter(typeof(UserFilter))]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly IInvestmentPlanService _investmentPlanService;
        private readonly IUserService _userService;
        private readonly IUpiDetailsService _upiDetailsService;
        private readonly IDepositService _depositService;
        private readonly IBankDetailsService _bankService;
        private readonly ITransactionDetailsService _transactionDetailsService;
        private readonly IInvestmentPlanService _investmentPlanService;
        private readonly IUserInvestmentService _userInvestmentService;
        private readonly ITransactionService _transactionService;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IUpiDetailsService upiDetailsService, IDepositService depositService, IBankDetailsService bankService, ITransactionDetailsService transactionDetailsService, IInvestmentPlanService investmentPlanService, IUserInvestmentService userInvestmentService,ITransactionService transactionService)
        {
            _logger = logger;
            //_investmentPlanService = investmentPlanService;
            _userService = userService;
            _upiDetailsService = upiDetailsService;
            _depositService = depositService;
            _bankService = bankService;
            _transactionDetailsService = transactionDetailsService;
            _investmentPlanService = investmentPlanService;
            _userInvestmentService = userInvestmentService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            User user = null;
            decimal balance = 0;

            if (!string.IsNullOrEmpty(userPhone))
            {
                try
                {
                    user = await _userService.GetByPhoneAsync(userPhone);
                    if (user != null)
                    {
                        balance = await _transactionService.CalculateUserBalanceAsync(user.Id);
                    }
                }
                catch (Exception ex)
                {
                    // Optional: log or handle error
                    Console.WriteLine("Error fetching user or balance: " + ex.Message);
                }
            }

            ViewBag.User = user;
            ViewBag.Balance = balance;
            return View(user);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Account()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");

            if (userPhone !=null)
            {
                ViewBag.UserData = await _transactionService.GetUserDetails(userPhone);
            }
            else
            {
                return RedirectToAction("Login", "User"); // Adjust as per your auth logic
            }
            
            return View();
        }
        public async Task<IActionResult> Investments()
        {
            //var plans = await _investmentPlanService.GetAllAsync();
            return View();
        }
        public async Task<IActionResult> Referrals()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            User user = null;

            if (!string.IsNullOrEmpty(userPhone))
            {
                try
                {
                    user = await _userService.GetByPhoneAsync(userPhone);
                    if (user != null)
                    {
                        ViewBag.ReferralData = await _userService.GetReferralDataAsync(user.Id);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching user: " + ex.Message);
                }
            }

            return View(user);
        }
        public async Task<IActionResult> TeamMembers()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            User user = null;
            TeamMemberDataDto teamMembers = new TeamMemberDataDto();

            if (!string.IsNullOrEmpty(userPhone))
            {
                try
                {
                    user = await _userService.GetByPhoneAsync(userPhone);
                    if (user != null)
                    {
                        teamMembers = await _userService.GetAllTeamMembersAsync(user.Id);
                        ViewBag.ReferralData = await _transactionService.ReferralAmuntAsync(user.Id);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching team members: " + ex.Message);
                }
            }

            return View(teamMembers);
        }
        public async Task<IActionResult> Recharge()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            User user = null;
            decimal balance = 0;

            if (!string.IsNullOrEmpty(userPhone))
            {
                try
                {
                    user = await _userService.GetByPhoneAsync(userPhone);
                    if (user != null)
                    {
                        balance = await _transactionService.CalculateUserBalanceAsync(user.Id);
                    }
                }
                catch (Exception ex)
                {
                    // Optional: log or handle error
                    Console.WriteLine("Error fetching user or balance: " + ex.Message);
                }
            }
            ViewBag.Balance = balance;
            return View();
        }
        public async Task<IActionResult> Payment(decimal? amount, string paymentType)
        {
            if (amount <= 0 || string.IsNullOrEmpty(paymentType))
            {
                return BadRequest("Invalid amount or payment type.");
            }

            string upiId = null;
            string qrCodeBase64 = null;
            BankDetails bankDetail = null;

            if (paymentType == "UPI")
            {
                var upiDetail = await _upiDetailsService.GetFirstActiveAdminUpiAsync();
                upiId = upiDetail?.UpiId ?? "No active admin UPI found";
                if (upiDetail != null)
                {
                    qrCodeBase64 = _upiDetailsService.GenerateUpiQrCode(upiId, (decimal)amount);
                }
            }
            else if (paymentType == "BankTransfer")
            {
                bankDetail = await _bankService.GetFirstActiveAdminBankAsync();
            }

            ViewBag.Amount = amount;
            ViewBag.PaymentType = paymentType;
            ViewBag.UpiId = upiId;
            ViewBag.QrCode = qrCodeBase64;
            ViewBag.BankDetail = bankDetail;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDeposit(string utrNumber, decimal amount, string paymentType, string upiId, string userId)
        {
            if (string.IsNullOrEmpty(utrNumber) || !new Regex("^[0-9A-Za-z]{6,12}$").IsMatch(utrNumber))
            {
                return Json(new { success = false, message = "Invalid UTR number. Must be 6-12 alphanumeric characters." });
            }
            var userPhone = HttpContext.Session.GetString("UserPhone");
            var user = !string.IsNullOrEmpty(userPhone) ? await _depositService.GetUserByPhoneAsync(userPhone) : null;
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var deposit = new Deposit
            {
                User = user,
                UserId = user.Id,
                Amount = amount,
                BonusPercentage = 0,
                Status = 0,
                PaymentType = paymentType == "UPI" ? 0 : 1,
                AdminUpiId = paymentType == "UPI" && upiId != "No active admin UPI found" ? (int?)_upiDetailsService.GetFirstActiveAdminUpiAsync().Result.Id : null,
                AdminAccountID = paymentType == "BankTransfer" ? (int?)_bankService.GetFirstActiveAdminBankAsync().Result.Id : null,
                UtrNumber = utrNumber,
                Date = DateTime.UtcNow
            };

            await _depositService.CreateDepositAsync(deposit);
            return Json(new { success = true, message = "Deposit submitted successfully." });
        }

        public async Task<IActionResult> Transactions()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "Account"); // Adjust as per your auth logic
            }

            var transactions = await _transactionDetailsService.GetUserTransactionsAsync(userPhone);
            return View(transactions);
        }

        public async Task<IActionResult> Invite()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User"); // Adjust as per your auth logic
            }
            var userdata = await _userService.GetByPhoneAsync(userPhone);
            var invite = await _userService.GetInviteData();
            ViewBag.UserData = userdata.ReferralCode;
            return View(invite);
        }
        public async Task<IActionResult> TransactionDetail(int id)
        {
            var transaction = await _transactionDetailsService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }
        public async Task<IActionResult> InvestmentPlans()
        {
            var investmentPlans = await _investmentPlanService.GetAllInvestmentPlansAsync();
            return View(investmentPlans);
        }

        [HttpPost]
        public async Task<IActionResult> Invest(int planId, int quantity, string utrNumber)
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            var user = !string.IsNullOrEmpty(userPhone) ? await _depositService.GetUserByPhoneAsync(userPhone) : null;
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Fetch user's current balance
            var balance = await _transactionService.CalculateUserBalanceAsync(user.Id);
            var plan = await _investmentPlanService.GetInvestmentPlanByIdAsync(planId);
            if (plan == null)
            {
                return Json(new { success = false, message = "Investment plan not found." });
            }

            decimal totalAmount = plan.InvestmentAmount * quantity;
            if (balance < totalAmount)
            {
                return Json(new { success = false, message = "Insufficient wallet balance." });
            }
            var setdays = 0;
            var userInvestment = new UserInvestment
            {
                UserId = user.Id,
                PlanId = planId,
                PurchaseQuantity = quantity,
                //StartDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow.AddDays(setdays),
                EndDate = DateTime.UtcNow.AddDays(int.TryParse(System.Text.RegularExpressions.Regex.Match(plan.RevenueDurationValue ?? "30", @"\d+").Value, out int days) ? days : 30),
                status = 1 // Purchased
            };

            // Update user's balance after investment
            balance -= totalAmount; // Deduct the investment amount
            await _userService.UpdateToPurchased(userPhone);
            await _userInvestmentService.CreateUserInvestmentAsync(userInvestment);

            return Json(new { success = true, message = "Investment successful. Wallet balance updated." });
        }
        // New action to show user investments
        public async Task<IActionResult> MyInvestments()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            var user = !string.IsNullOrEmpty(userPhone) ? await _depositService.GetUserByPhoneAsync(userPhone) : null;
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var investments = await _userInvestmentService.GetUserInvestmentsByUserIdAsync(user.Id);
            var viewModel = investments.Select(investment => new InvestmentViewModel
            {
                Investment = investment,
                InvestedAmount = investment.InvestmentPlan.InvestmentAmount * investment.PurchaseQuantity,
                CalculatedAmount = CalculateAccruedProfit(investment)
            }).ToList();

            return View(viewModel);
        }
        private decimal CalculateAccruedProfit(UserInvestment investment)
        {
            //var hoursElapsed = (DateTime.UtcNow - investment.StartDate).TotalHours;
            //var fullDays = (int)(hoursElapsed / 24);
            var dailyEarnings = investment.InvestmentPlan.DailyEarningsPerUnit ?? 0;
            return dailyEarnings * investment.PurchaseQuantity * int.Parse(investment.InvestmentPlan.RevenueDurationValue);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear authentication cookies
            Response.Cookies.Delete("rareentUser", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("rareentAdmin", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("findUser", new CookieOptions { Path = "/" });

            // Clear session if needed
            HttpContext.Session.Clear();

            // Optionally, clear any other server-side session/authentication data

            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> AllTransactions()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
                return RedirectToAction("Login");

            var user = await _depositService.GetUserByPhoneAsync(userPhone);
            if (user == null)
                return RedirectToAction("Login");

            var viewModel = await _transactionService.GetUserTransactionsAsync(user.Id);
            return View(viewModel);
        }

    }
}
