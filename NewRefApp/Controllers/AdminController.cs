using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Models;
using NewRefApp.Services;

namespace NewRefApp.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class AdminController : Controller
    {
        private readonly IDepositService _depositService;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public AdminController(IDepositService depositService, ApplicationDbContext context, IUserService userService)
        {
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            _depositService = depositService;
            _context = context;
            _userService = userService;
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
                        balance = await _depositService.CalculateUserBalanceAsync(user.Id);
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
        public async Task<IActionResult> SettleTransactions()
        {
            var deposits = await _depositService.GetAllDepositsAsync();
            return View(deposits);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveTransaction(int id)
        {
            var deposit = await _depositService.GetDepositByIdAsync(id);
            if (deposit == null)
            {
                return Json(new { success = false, message = "Transaction not found." });
            }

            deposit.Status = 1; // Approve (Completed)
            await _depositService.UpdateDepositAsync(deposit);
            return Json(new { success = true, message = "Transaction approved successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> CancelTransaction(int id)
        {
            var deposit = await _depositService.GetDepositByIdAsync(id);
            if (deposit == null)
            {
                return Json(new { success = false, message = "Transaction not found." });
            }

            deposit.Status = 2; // Cancel (Failed)
            await _depositService.UpdateDepositAsync(deposit);
            return Json(new { success = true, message = "Transaction canceled successfully." });
        }

        // SettleWithdraw action
        public async Task<IActionResult> SettleWithdraw()
        {
            var withdrawals = await _context.Withdraw.Where(w => w.Status == 0).ToListAsync(); // Show only pending withdrawals
            return View(withdrawals);
        }

        [HttpPost]
        public async Task<IActionResult> ShowSuccessWithdrawConfirmation(int id)
        {
            return Json(new { success = true, id = id });
        }

        [HttpPost]
        public async Task<IActionResult> SuccessWithdraw(int id)
        {
            var withdraw = await _context.Withdraw.FindAsync(id);
            if (withdraw != null)
            {
                withdraw.Status = 1; // Completed
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("SettleWithdraw");
        }

        [HttpPost]
        public async Task<IActionResult> ShowCancelWithdrawConfirmation(int id)
        {
            return Json(new { success = true, id = id });
        }

        [HttpPost]
        public async Task<IActionResult> CancelWithdraw(int id)
        {
            var withdraw = await _context.Withdraw.FindAsync(id);
            if (withdraw != null)
            {
                withdraw.Status = 2; // Failed
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("SettleWithdraw");
        }

        //// SuccessfulWithdraws action
        //public async Task<IActionResult> SuccessfulWithdraws()
        //{
        //    var successfulWithdraws = await _withdrawService.GetSuccessfulWithdrawsAsync();
        //    var successfulDeposits = await _transactionService.GetSuccessfulTransactionsAsync();
        //    ViewBag.SuccessfulWithdraws = successfulWithdraws;
        //    ViewBag.SuccessfulDeposits = successfulDeposits;
        //    return View();
        //}
    }
}
