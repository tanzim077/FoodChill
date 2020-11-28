using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FoodChill.Models;
using FoodChill.Data;
using FoodChill.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FoodChill.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(s => s.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };
            return View(indexVM);
        }

        [Authorize]
        //GET Details
        public async Task<IActionResult> Details(int id)
        {
            
            var menuItemFromDB = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.ID == id).FirstOrDefaultAsync();
            ShoppingCart cartobj = new ShoppingCart
            {
                MenuItem = menuItemFromDB,
                MenuItemID = menuItemFromDB.ID
            };
            return View(cartobj);
        }

        //POST Details
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartiobj)
        {
            cartiobj.ID = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartiobj.ApplicationUserID = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserID == cartiobj.ApplicationUserID && c.MenuItemID == cartiobj.MenuItemID).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(cartiobj);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + cartiobj.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(c => c.ApplicationUserID == cartiobj.ApplicationUserID).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItemFromDB = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.ID == cartiobj.MenuItemID).FirstOrDefaultAsync();
                ShoppingCart cartobj = new ShoppingCart
                {
                    MenuItem = menuItemFromDB,
                    MenuItemID = menuItemFromDB.ID
                };
                return View(cartobj);
            }
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
