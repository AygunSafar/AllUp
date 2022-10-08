using AllUp.DAL;
using AllUp.Helpers;
using AllUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace AllUp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CategoriesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        #region Index
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Categories.Include(x => x.Children).Include(x => x.Parent).ToListAsync();
            return View(categories);
        }
        #endregion

        #region CreateGetMethod
        public async Task<IActionResult> Create()
        {
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            return View();
        }

        #endregion

        #region CreatePost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, int? mainCatId)
        {
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();


            if (category.IsMain)
            {
                bool isExist = await _db.Categories.AnyAsync(x => x.Parent.Name == category.Name);
                if (isExist)
                {
                    ModelState.AddModelError("Title", "This service is already exist");
                    return View();
                }

                if (category.Photo == null)
                {
                    ModelState.AddModelError("Photo", "Zehmet olmasa bir sekil elave edin");
                    return View();
                }
                if (!category.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", " sekil elave edin");
                    return View();
                }
                if (category.Photo.IsOlder1MB())
                {
                    ModelState.AddModelError("Photo", " 1 mb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images");
                category.Image = await category.Photo.SaveFileAsync(folder);
            }
            else
            {
                if (mainCatId == null)
                {
                    return NotFound();
                }
                category.ParentId = mainCatId;

            }


            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion

        #region DetailMehtod

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            };
            Category category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return BadRequest();
            };
            return View(category);
        }


        #endregion

    }
}
