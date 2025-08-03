using Microsoft.AspNetCore.Mvc;
using NewRefApp.Data.DTOs;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Models;

namespace NewRefApp.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class InvestmentPlanController : Controller
    {
        private readonly IInvestmentPlanService _investmentPlanService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InvestmentPlanController(IInvestmentPlanService investmentPlanService, IWebHostEnvironment webHostEnvironment)
        {
            _investmentPlanService = investmentPlanService;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create(InvestmentPlanViewModel model)
        {
            
                string uniqueFileName = null;
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var plan = new InvestmentPlan
                {
                    Name = model.Name,
                    Category = model.Category,
                    RevenueDurationValue = model.RevenueDurationValue,
                    VipLevel = model.VipLevel,
                    DailyEarningsPerUnit = model.DailyEarningsPerUnit,
                    HourlyEarningsPerUnit = model.HourlyEarningsPerUnit,
                    InvestmentAmount = model.InvestmentAmount,
                    ImagePath = uniqueFileName != null ? "/images/" + uniqueFileName : null
                };

                await _investmentPlanService.CreateInvestmentPlanAsync(plan);
                return RedirectToAction(nameof(Index));
           
        }

        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _investmentPlanService.GetInvestmentPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            var viewModel = new InvestmentPlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Category = plan.Category,
                RevenueDurationValue = plan.RevenueDurationValue,
                VipLevel = plan.VipLevel,
                DailyEarningsPerUnit = plan.DailyEarningsPerUnit,
                HourlyEarningsPerUnit = plan.HourlyEarningsPerUnit,
                InvestmentAmount = plan.InvestmentAmount,
                ExistingImagePath = plan.ImagePath
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvestmentPlanViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            
                string uniqueFileName = model.ExistingImagePath;

                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var plan = new InvestmentPlan
                {
                    Id = model.Id,
                    Name = model.Name,
                    Category = model.Category,
                    RevenueDurationValue = model.RevenueDurationValue,
                    VipLevel = model.VipLevel,
                    DailyEarningsPerUnit = model.DailyEarningsPerUnit,
                    HourlyEarningsPerUnit = model.HourlyEarningsPerUnit,
                    InvestmentAmount = model.InvestmentAmount,
                    ImagePath = uniqueFileName != null ? "/images/" + uniqueFileName : null
                };

                await _investmentPlanService.UpdateInvestmentPlanAsync(plan);
                return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _investmentPlanService.GetInvestmentPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _investmentPlanService.GetInvestmentPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            // Delete image file if exists
            if (!string.IsNullOrEmpty(plan.ImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", plan.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _investmentPlanService.DeleteInvestmentPlanAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }

}
