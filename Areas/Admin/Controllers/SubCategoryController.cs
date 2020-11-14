using FoodChill.Data;
using FoodChill.Models;
using FoodChill.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodChill.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var subCatogry = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCatogry);
        }

        //GET Create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        //POST Sub-Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubcategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.ID == model.SubCategory.CategoryID);
                if (doesSubcategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error :  Subcategory already exists in " + doesSubcategoryExists.First().Category.Name + ". Please use another name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
            }

                SubCategoryAndCategoryViewModel subCategoryAndCategoryVM = new SubCategoryAndCategoryViewModel()
                {
                    CategoryList = await _db.Category.ToListAsync(),
                    SubCategory = model.SubCategory,
                    SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                    StatusMessage = StatusMessage
                };

                return View(subCategoryAndCategoryVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from SubCategory in _db.SubCategory
                             where SubCategory.CategoryID == id
                             select SubCategory).ToListAsync();
            return Json(new SelectList(subCategories, "ID", "Name")); // Pass ID and Name property in json for display
        }
    }

}
