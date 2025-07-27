using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewRefApp.Data;
using NewRefApp.Interfaces;
using NewRefApp.Middlewares;
using NewRefApp.Models;
using NewRefApp.Services;

namespace NewRefApp.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class UpiDetailsController : Controller
    {
        private readonly IUpiDetailsService _upiDetailsService;
        private readonly IUserService _userService;

        public UpiDetailsController(IUpiDetailsService upiDetailsService, IUserService userService)
        {
            _upiDetailsService = upiDetailsService;
            _userService = userService;
        }

        // GET: UpiDetails
        public async Task<IActionResult> Index()
        {
            var upiDetails = await _upiDetailsService.GetAllUpiDetailsAsync();
            return View(upiDetails);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UpiDetails upiDetails)
        {

            var userPhone = HttpContext.Session.GetString("UserPhone");
            if (!string.IsNullOrEmpty(userPhone))
            {
                try
                {
                    var user = await _userService.GetByPhoneAsync(userPhone); // Assume this method exists
                    if (user != null)
                    {
                        upiDetails.UserId = user.Id;
                        upiDetails.IsAdmin = user.IsAdmin; // Set based on user role
                        //upiDetails.Status = true; // Set status based on user activity, default to true
                    }
                    else
                    {
                        upiDetails.UserId = 1; // Fallback UserId if user not found
                        upiDetails.IsAdmin = false; // Default to non-admin
                        upiDetails.Status = true; // Default to active
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine("Error fetching user: " + ex.Message);
                    upiDetails.UserId = 1; // Fallback
                    upiDetails.IsAdmin = false;
                    upiDetails.Status = false;
                }
                await _upiDetailsService.CreateUpiDetailAsync(upiDetails);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //upiDetails.UserId = 1; // Fallback if no session
                //upiDetails.IsAdmin = false;
                //upiDetails.Status = true;
                return View(upiDetails);
            }



        }

        public async Task<IActionResult> Details(int id)
        {
            var upiDetail = await _upiDetailsService.GetUpiDetailByIdAsync(id);
            if (upiDetail == null)
            {
                return NotFound();
            }
            return View(upiDetail);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var upiDetail = await _upiDetailsService.GetUpiDetailByIdAsync(id);
            if (upiDetail == null)
            {
                return NotFound();
            }
            return View(upiDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpiDetails upiDetails)
        {
            if (id != upiDetails.Id)
            {
                return NotFound();
            }

            var userPhone = HttpContext.Session.GetString("UserPhone");
            var user = !string.IsNullOrEmpty(userPhone) ? await _userService.GetByPhoneAsync(userPhone) : null;
            upiDetails.UserId = user.Id;
            await _upiDetailsService.UpdateUpiDetailAsync(upiDetails);
                return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var upiDetail = await _upiDetailsService.GetUpiDetailByIdAsync(id);
            if (upiDetail == null)
            {
                return NotFound();
            }
            return View(upiDetail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _upiDetailsService.DeleteUpiDetailAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
