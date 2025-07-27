using Microsoft.AspNetCore.Mvc;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Models;
using NewRefApp.Services;

namespace NewRefApp.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class BankDetailsController : Controller
    {
        private readonly IBankDetailsService _bankService;
        private readonly IUserService _userService;

        public BankDetailsController(IBankDetailsService bankService,IUserService userService)
        {
            _userService = userService;
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
            try
            {
                var userPhone = HttpContext.Session.GetString("UserPhone");
                var user = !string.IsNullOrEmpty(userPhone) ? await _userService.GetByPhoneAsync(userPhone) : null;
                bankDetails.UserId = user.Id; // Replace with actual user logic
                await _bankService.CreateBankDetailAsync(bankDetails, true);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                throw;
            }

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
            try
            {
                if (id != bankDetails.Id)
                {
                    return NotFound();
                }

                var userPhone = HttpContext.Session.GetString("UserPhone");
                var user = !string.IsNullOrEmpty(userPhone) ? await _userService.GetByPhoneAsync(userPhone) : null;
                bankDetails.UserId = user.Id;
                await _bankService.UpdateBankDetailAsync(bankDetails);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                throw;
            }


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
