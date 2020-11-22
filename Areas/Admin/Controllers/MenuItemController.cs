using FoodChill.Data;
using FoodChill.Models;
using FoodChill.Models.ViewModels;
using FoodChill.Utility;
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
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment; //saving Images directly databse

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;

            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category,
                MenuItem = new Models.MenuItem()
            };
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).ToListAsync();
            return View(menuItems);
        }

        //GEt -- Create
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST -- Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["subCategoryID"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }
            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Work on image saving section

            string webrootpath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menutemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.ID);

            if (files.Count > 0)
            {
                //file uploaded
                var upload = Path.Combine(webrootpath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(upload, MenuItemVM.MenuItem.ID + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menutemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.ID + extension;

            }
            else
            {
                //work with default file
                var upload = Path.Combine(webrootpath, @"images\" + SD.DefaultImage);
                System.IO.File.Copy(upload, webrootpath + @"\images\" + MenuItemVM.MenuItem.ID + ".png");
                menutemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.ID + ".png";
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GEt -- Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ID == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryID == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        //POST -- Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int id)
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["subCategoryID"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryID == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            //Work on image saving section

            string webrootpath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menutemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.ID);

            if (files.Count > 0)
            {
                //new file uploaded
                var upload = Path.Combine(webrootpath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete Previous file

                var imagePath = Path.Combine(webrootpath, menutemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //Upload new files
                using (var filesStream = new FileStream(Path.Combine(upload, MenuItemVM.MenuItem.ID + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menutemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.ID + extension_new;

            }

            menutemFromDb.Name = MenuItemVM.MenuItem.Name;
            menutemFromDb.Price = MenuItemVM.MenuItem.Price;
            menutemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menutemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
            menutemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menutemFromDb.Description = MenuItemVM.MenuItem.Description;


            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GEt -- Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ID == id);
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        //GET -- Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.ID == id);
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        //POST -- Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            string webrootpath = _webHostEnvironment.WebRootPath;
            MenuItem menuItem = await _db.MenuItem.FindAsync(id);

            if (menuItem != null)
            {
                var imagepath = Path.Combine(webrootpath, menuItem.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                _db.MenuItem.Remove(menuItem);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
    }

}
