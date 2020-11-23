using FoodChill.Data;
using FoodChill.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoodChill.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET -- Index
        public async Task<IActionResult> Index()
        {
            var model = await _db.Coupon.ToListAsync();
            return View(model);
        }

        //GET -- Create
        public IActionResult Create()
        {
            return View();
        }

        //POST -- Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] cpnPictur = null;
                    using (var fileStream = files[0].OpenReadStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            cpnPictur = memoryStream.ToArray();
                        }
                    }
                    coupons.Picture = cpnPictur;
                }
                _db.Coupon.Add(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        //GET -- Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var CouponItem = await _db.Coupon.SingleOrDefaultAsync(m => m.ID == id);
            if (CouponItem == null)
            {
                return NotFound();
            }
            return View(CouponItem);
        }

        //POST -- Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupons)
        {
            if (coupons.ID == 0)
            {
                return NotFound();
            }

            var couponFromDb = await _db.Coupon.Where(c => c.ID == coupons.ID).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] cpnPictur = null;
                    using (var fileStream = files[0].OpenReadStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            cpnPictur = memoryStream.ToArray();
                        }
                    }
                    couponFromDb.Picture = cpnPictur;
                }

                couponFromDb.Name = coupons.Name;
                couponFromDb.MinimumAmount = coupons.MinimumAmount;
                couponFromDb.Discount = coupons.Discount;
                couponFromDb.CouponType = coupons.CouponType;
                couponFromDb.IsActive = coupons.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        //GET -- Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Coupon = await _db.Coupon.SingleOrDefaultAsync(m => m.ID == id);
            if (Coupon == null)
            {
                return NotFound();
            }
            return View(Coupon);
        }

        //GET -- Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.SingleOrDefaultAsync(c => c.ID == id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        //POST -- Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _db.Coupon.SingleOrDefaultAsync(c => c.ID == id);
            _db.Coupon.Remove(coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
