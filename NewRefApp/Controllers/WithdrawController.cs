using Microsoft.AspNetCore.Mvc;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;
using NewRefApp.Models;
using NewRefApp.Services;

namespace NewRefApp.Controllers
{
    public class WithdrawController : Controller
    {
        private readonly IDepositService _depositService;
        private readonly IWithdrawService _withdrawService;
        private readonly IUserService _userService;
        private readonly IBankDetailsService _bankDetailsService;
        private readonly IUpiDetailsService _upiDetailsService;

        public WithdrawController(IDepositService depositService, IWithdrawService withdrawService, IUserService userService, IBankDetailsService bankDetailsService, IUpiDetailsService upiDetailsService)
        {
            _depositService = depositService;
            _withdrawService = withdrawService;
            _userService = userService;
            _bankDetailsService = bankDetailsService;
            _upiDetailsService = upiDetailsService;
        }

        // Create both bank and UPI details in a single view
        public async Task<IActionResult> CreateAccount()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var bankDetails = await _bankDetailsService.GetBankDetailsByUserIdAsync(user.Id);
            var upiDetails = await _upiDetailsService.GetUpiDetailsByUserIdAsync(user.Id);
            if (bankDetails != null && upiDetails != null)
            {
                return RedirectToAction("AccountDetails");
            }
            return View(new AccountDetailsViewModel { BankDetails = new BankDetails { UserId = user.Id }, UpiDetails = new UpiDetails { UserId = user.Id, Status = true } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(AccountDetailsViewModel model)
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            model.BankDetails.UserId = user.Id;
            model.UpiDetails.UserId = user.Id;
            model.UpiDetails.Status = true; // Set as active for user

            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            await _bankDetailsService.CreateBankDetailAsync(model.BankDetails);

            // Create or update Withdraw record with transactionPassword
            var withdraw = await _withdrawService.GetWithdrawByUserIdAsync(user.Id) ?? new Withdraw { UserId = user.Id };
            withdraw.transactionPassword = model.TransactionPassword; // Set withdrawal password
            withdraw.Status = 0; // Pending by default
            withdraw.Date = DateTime.UtcNow;
            if (await _withdrawService.GetWithdrawByUserIdAsync(user.Id) == null)
            {
                await _withdrawService.CreateWithdrawAsync(withdraw);
            }
            else
            {
                await _withdrawService.UpdateWithdrawAsync(withdraw);
            }

            await _upiDetailsService.CreateUpiDetailAsync(model.UpiDetails);
            return RedirectToAction("AccountDetails");
        }

        // View and edit account details
        public async Task<IActionResult> AccountDetails()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var bankDetails = await _bankDetailsService.GetBankDetailsByUserIdAsync(user.Id);
            var upiDetails = await _upiDetailsService.GetUpiDetailsByUserIdAsync(user.Id);
            if (bankDetails == null || upiDetails == null)
            {
                return RedirectToAction("CreateAccount");
            }
            return View(new AccountDetailsViewModel { BankDetails = bankDetails, UpiDetails = upiDetails });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBankDetails(AccountDetailsViewModel model)
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var existingBankDetails = await _bankDetailsService.GetBankDetailsByUserIdAsync(user.Id);
            if (existingBankDetails == null)
            {
                return RedirectToAction("CreateAccount");
            }

            if (!ModelState.IsValid)
            {
                model.UpiDetails = await _upiDetailsService.GetUpiDetailsByUserIdAsync(user.Id);
                return View("AccountDetails", model);
            }

            existingBankDetails.AccountNumber = model.BankDetails.AccountNumber;
            existingBankDetails.IFSCCode = model.BankDetails.IFSCCode;
            existingBankDetails.AccountName = model.BankDetails.AccountName;
            existingBankDetails.BankName = model.BankDetails.BankName;
            existingBankDetails.BankLocation = model.BankDetails.BankLocation;
            await _bankDetailsService.UpdateBankDetailAsync(existingBankDetails);
            return RedirectToAction("AccountDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUpiDetails(AccountDetailsViewModel model)
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var existingUpiDetails = await _upiDetailsService.GetUpiDetailsByUserIdAsync(user.Id);
            if (existingUpiDetails == null)
            {
                return RedirectToAction("CreateAccount");
            }

            if (!ModelState.IsValid)
            {
                model.BankDetails = await _bankDetailsService.GetBankDetailsByUserIdAsync(user.Id);
                return View("AccountDetails", model);
            }

            existingUpiDetails.UpiId = model.UpiDetails.UpiId;
            existingUpiDetails.Status = true; // Ensure active status for user
            await _upiDetailsService.UpdateUpiDetailAsync(existingUpiDetails);
            return RedirectToAction("AccountDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWithdrawPassword(AccountDetailsViewModel model)
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var withdraw = await _withdrawService.GetWithdrawByUserIdAsync(user.Id);
            if (withdraw == null)
            {
                withdraw = new Withdraw { UserId = user.Id };
                await _withdrawService.CreateWithdrawAsync(withdraw);
            }

            if (!ModelState.IsValid)
            {
                model.BankDetails = await _bankDetailsService.GetBankDetailsByUserIdAsync(user.Id);
                model.UpiDetails = await _upiDetailsService.GetUpiDetailsByUserIdAsync(user.Id);
                return View("AccountDetails", model);
            }

            withdraw.transactionPassword = model.TransactionPassword;
            withdraw.Status = 0; // Pending by default
            withdraw.Date = DateTime.UtcNow;
            await _withdrawService.UpdateWithdrawAsync(withdraw);

            return RedirectToAction("AccountDetails");
        }

        // Initiate withdrawal
        public async Task<IActionResult> WithdrawMoney()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var balance = await _depositService.CalculateUserBalanceAsync(user.Id);
            var bankDetails = await _bankDetailsService.GetBankDetailsByUserIdAsync(user.Id);
            var upiDetails = await _upiDetailsService.GetUpiDetailsByUserIdAsync(user.Id);
            if (bankDetails == null || upiDetails == null)
            {
                return RedirectToAction("CreateAccount");
            }

            var withdraw = await _withdrawService.GetWithdrawByUserIdAsync(user.Id) ?? new Withdraw { UserId = user.Id };
            ViewBag.Balance = balance;
            ViewBag.BankDetails = bankDetails;
            ViewBag.UpiDetails = upiDetails;
            return View(withdraw);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawMoney(Withdraw withdraw)
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var balance = await _depositService.CalculateUserBalanceAsync(user.Id);
            var bankDetails = await _bankDetailsService.GetBankDetailsByUserIdAsync(user.Id);
            var upiDetails = await _upiDetailsService.GetUpiDetailsByUserIdAsync(user.Id);
            if (bankDetails == null || upiDetails == null)
            {
                return RedirectToAction("CreateAccount");
            }

            var existingWithdraw = await _withdrawService.GetWithdrawByUserIdAsync(user.Id);
            if (existingWithdraw == null || existingWithdraw.transactionPassword != withdraw.transactionPassword)
            {
                ModelState.AddModelError("transactionPassword", "Invalid transaction password.");
                ViewBag.Balance = balance;
                ViewBag.BankDetails = bankDetails;
                ViewBag.UpiDetails = upiDetails;
                return View(withdraw);
            }

            if (balance < withdraw.Amount)
            {
                ModelState.AddModelError("Amount", "Insufficient balance.");
                ViewBag.Balance = balance;
                ViewBag.BankDetails = bankDetails;
                ViewBag.UpiDetails = upiDetails;
                return View(withdraw);
            }

            withdraw.UserId = user.Id;
            withdraw.PaymentType = withdraw.PaymentType == 0 ? 0 : 1; // Ensure valid PaymentType
            withdraw.BankDetailId = withdraw.PaymentType == 1 ? bankDetails.Id : null;
            withdraw.UpiDetailId = withdraw.PaymentType == 0 ? upiDetails.Id : null;
            withdraw.Status = 0; // Pending
            withdraw.Date = DateTime.UtcNow;
            withdraw.transactionPassword = existingWithdraw.transactionPassword; // Preserve the password

            if (existingWithdraw == null)
            {
                await _withdrawService.CreateWithdrawAsync(withdraw);
            }
            else
            {
                existingWithdraw.Amount = withdraw.Amount;
                existingWithdraw.PaymentType = withdraw.PaymentType;
                existingWithdraw.BankDetailId = withdraw.BankDetailId;
                existingWithdraw.UpiDetailId = withdraw.UpiDetailId;
                existingWithdraw.Status = 0;
                await _withdrawService.UpdateWithdrawAsync(existingWithdraw);
            }

            return RedirectToAction("WithdrawalHistory");
        }

        // View withdrawal history
        public async Task<IActionResult> WithdrawalHistory()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (string.IsNullOrEmpty(userPhone))
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetByPhoneAsync(userPhone);
            var withdrawals = await _withdrawService.GetWithdrawalsByUserIdAsync(user.Id);
            return View(withdrawals);
        }
    }
}
