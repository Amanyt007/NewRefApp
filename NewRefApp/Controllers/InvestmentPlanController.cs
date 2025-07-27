using Microsoft.AspNetCore.Mvc;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Models;

namespace NewRefApp.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class InvestmentPlanController : Controller
    {
        private readonly IInvestmentPlanService _investmentPlanService;

        public InvestmentPlanController(IInvestmentPlanService investmentPlanService)
        {
            _investmentPlanService = investmentPlanService;
        }

        public async Task<IActionResult> Index()
        {
            var investmentPlans = await _investmentPlanService.GetAllInvestmentPlansAsync();
            return View(investmentPlans);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvestmentPlan investmentPlan)
        {
            if (ModelState.IsValid)
            {
                await _investmentPlanService.CreateInvestmentPlanAsync(investmentPlan);
                return RedirectToAction(nameof(Index));
            }
            return View(investmentPlan);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var investmentPlan = await _investmentPlanService.GetInvestmentPlanByIdAsync(id);
            if (investmentPlan == null)
            {
                return NotFound();
            }
            return View(investmentPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvestmentPlan investmentPlan)
        {
            if (id != investmentPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _investmentPlanService.UpdateInvestmentPlanAsync(investmentPlan);
                return RedirectToAction(nameof(Index));
            }
            return View(investmentPlan);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var investmentPlan = await _investmentPlanService.GetInvestmentPlanByIdAsync(id);
            if (investmentPlan == null)
            {
                return NotFound();
            }
            return View(investmentPlan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _investmentPlanService.DeleteInvestmentPlanAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
