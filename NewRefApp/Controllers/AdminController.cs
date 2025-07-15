using Microsoft.AspNetCore.Mvc;
using NewRefApp.Interfaces;

namespace NewRefApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDepositService _depositService;

        public AdminController(IDepositService depositService)
        {
            _depositService = depositService;
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
    }
}
