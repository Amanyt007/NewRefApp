using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Models;
using NewRefApp.Services;

namespace NewRefApp.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IDepositService _depositService;
        private readonly IUserService _userService;
        private readonly ITransactionService _transactionService;

        public AdminController(IAdminService adminService, IDepositService depositService, IUserService userService, ITransactionService transactionService)
        {
            _adminService = adminService;
            _depositService = depositService;
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            _userService = userService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var userPhone = HttpContext.Session.GetString("UserPhone");

            if (string.IsNullOrEmpty(userPhone))
                return RedirectToAction("Login", "Admin");

            var user = await _adminService.GetLoggedInUserAsync(userPhone);
            if (user == null)
                return RedirectToAction("Login", "Admin");

            var balance = await _adminService.GetUserBalanceAsync(user.Id);

            ViewBag.User = user;
            ViewBag.Balance = balance;
            return View(user);
        }

        public async Task<IActionResult> SettleTransactions()
        {
            var deposits = await _adminService.GetPendingDepositsAsync();
            return View(deposits);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveTransaction(int id)
        {
            var deposit = await _depositService.GetDepositByIdAsync(id);
            if (deposit == null)
                return Json(new { success = false, message = "Transaction not found." });

            deposit.Status = 1; // Approved
            await _depositService.UpdateDepositAsync(deposit);
            return Json(new { success = true, message = "Transaction approved successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> CancelTransaction(int id)
        {
            var deposit = await _depositService.GetDepositByIdAsync(id);
            if (deposit == null)
                return Json(new { success = false, message = "Transaction not found." });

            deposit.Status = 2; // Cancelled
            await _depositService.UpdateDepositAsync(deposit);
            return Json(new { success = true, message = "Transaction canceled successfully." });
        }

        //public async Task<IActionResult> SettleWithdraw()
        //{
        //    var withdrawals = await _adminService.GetPendingWithdrawsAsync();
        //    return View(withdrawals);
        //}
        public async Task<IActionResult> SettleWithdraw(int? statusFilter, string phoneSearch)
        {
            // get data
            var withdrawals = await _adminService.GetPendingWithdrawsAsync(statusFilter, phoneSearch);

            // keep selected filter in ViewBag
            ViewBag.StatusFilter = statusFilter;
            ViewBag.PhoneSearch = phoneSearch;

            // dropdown options
            ViewBag.StatusOptions = new SelectList(new[]
            {
        new { Value = "", Text = "All" },
        new { Value = "0", Text = "Pending" },
        new { Value = "1", Text = "Done" },
        new { Value = "2", Text = "Failed" }
    }, "Value", "Text", statusFilter);

            return View(withdrawals);
        }




        [HttpPost]
        public async Task<IActionResult> ShowSuccessWithdrawConfirmation(int id)
        {
            var withdraw = await _adminService.GetWithdrawByIdAsync(id);
            if (withdraw != null)
            {
                withdraw.Status = 1; // Completed
                await _depositService.SaveChangesAsync(); // Add this to DepositService
            }
            return RedirectToAction("SettleWithdraw");
        }

        [HttpPost]
        public async Task<IActionResult> ShowCancelWithdrawConfirmation(int id)
        {
            var withdraw = await _adminService.GetWithdrawByIdAsync(id);
            if (withdraw != null)
            {
                withdraw.Status = 2; // Failed
                await _depositService.SaveChangesAsync(); // Use same SaveChangesAsync method
            }
            return RedirectToAction("SettleWithdraw");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("rareentUser", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("rareentAdmin", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("findUser", new CookieOptions { Path = "/" });
            HttpContext.Session.Clear();

            return Json(new { success = true });
        }

        public async Task<IActionResult> AllUsers(string filter)
        {
            var users = string.IsNullOrEmpty(filter)
                ? await _adminService.GetAllUsersAsync()
                : await _adminService.GetFilteredUsersAsync(filter);

            ViewBag.Filter = filter;
            return View(users);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var transactions = await _transactionService.GetUserTransactionsAsync(user.Id);

            var viewModel = new UserDetailWithTransactionsViewModel
            {
                User = user,
                TransactionsData = transactions
            };

            return View(viewModel);
        }

    }
}
