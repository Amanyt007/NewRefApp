using Microsoft.AspNetCore.Mvc;
using NewRefApp.Interfaces;
using NewRefApp.Models;

namespace NewRefApp.Controllers
{
    public class BankDetailsController : Controller
    {
        private readonly IBankDetailsService _bankService;

        public BankDetailsController(IBankDetailsService bankService)
        {
            _bankService = bankService;
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
        }

        public async Task<IActionResult> Index()
        {
            var bankDetails = await _bankService.GetAllBankDetailsAsync();
            return View(bankDetails);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BankDetails bankDetails)
        {
            if (ModelState.IsValid)
            {
                bankDetails.UserId = 1; // Replace with actual user logic
                await _bankService.CreateBankDetailAsync(bankDetails);
                return RedirectToAction(nameof(Index));
            }
            return View(bankDetails);
        }

        public async Task<IActionResult> Details(int id)
        {
            var bankDetail = await _bankService.GetBankDetailByIdAsync(id);
            if (bankDetail == null)
            {
                return NotFound();
            }
            return View(bankDetail);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var bankDetail = await _bankService.GetBankDetailByIdAsync(id);
            if (bankDetail == null)
            {
                return NotFound();
            }
            return View(bankDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BankDetails bankDetails)
        {
            if (id != bankDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _bankService.UpdateBankDetailAsync(bankDetails);
                return RedirectToAction(nameof(Index));
            }
            return View(bankDetails);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var bankDetail = await _bankService.GetBankDetailByIdAsync(id);
            if (bankDetail == null)
            {
                return NotFound();
            }
            return View(bankDetail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bankService.DeleteBankDetailAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
