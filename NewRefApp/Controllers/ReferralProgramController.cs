using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Models;

namespace NewRefApp.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(AdminFilter))]
    public class ReferralProgramController : Controller
    {
        private readonly IReferralProgramService _referralProgramService;

        public ReferralProgramController(IReferralProgramService referralProgramService)
        {
            _referralProgramService = referralProgramService;
        }

        // Index: List all referral programs
        public async Task<IActionResult> Index()
        {
            var referralPrograms = await _referralProgramService.GetAllReferralProgramsAsync();
            return View(referralPrograms);
        }

        // Details: View a specific referral program
        public async Task<IActionResult> Details(int id)
        {
            var referralProgram = await _referralProgramService.GetReferralProgramByIdAsync(id);
            if (referralProgram == null)
            {
                return NotFound();
            }
            return View(referralProgram);
        }

        // Create: Get and Post actions
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReferralProgram referralProgram)
        {
            if (ModelState.IsValid)
            {
                await _referralProgramService.CreateReferralProgramAsync(referralProgram);
                return RedirectToAction(nameof(Index));
            }
            return View(referralProgram);
        }

        // Edit: Get and Post actions
        public async Task<IActionResult> Edit(int id)
        {
            var referralProgram = await _referralProgramService.GetReferralProgramByIdAsync(id);
            if (referralProgram == null)
            {
                return NotFound();
            }
            return View(referralProgram);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReferralProgram referralProgram)
        {
            if (id != referralProgram.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _referralProgramService.UpdateReferralProgramAsync(referralProgram);
                return RedirectToAction(nameof(Index));
            }
            return View(referralProgram);
        }

        // Delete: Get and Post actions
        public async Task<IActionResult> Delete(int id)
        {
            var referralProgram = await _referralProgramService.GetReferralProgramByIdAsync(id);
            if (referralProgram == null)
            {
                return NotFound();
            }
            return View(referralProgram);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _referralProgramService.DeleteReferralProgramAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
