using AllUp.DAL;
using AllUp.Helpers;
using AllUp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AllUp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _db.Products.Include(x => x.ProductImages).Include(x => x.ProductDetail).Include(x => x.ProductCategories).ThenInclude(x => x.Category).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            Category? firstMainCategory = await _db.Categories.Include(x => x.Children).FirstOrDefaultAsync(x => x.IsMain);
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            ViewBag.ChildCategories = firstMainCategory.Children;

            return View();
        }
        public async Task<IActionResult> LoadChildCategories(int mainId)
        {
            List<Category> childCategories = await _db.Categories.Where(x => x.ParentId == mainId).ToListAsync();
            return PartialView("_LoadChildCategoriesPartial", childCategories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int? mainCatId, int? childCatId)
        {
            Category? firstMainCategory = await _db.Categories.Include(x => x.Children).FirstOrDefaultAsync(x => x.IsMain);
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();
            ViewBag.ChildCategories = firstMainCategory.Children;
            if (product.Photos == null)
            {
                ModelState.AddModelError("Photos", "Zehmet olmasa sekil yukleyin");
                return View();
            }
            if (mainCatId == null)
            {
                return BadRequest();
            }
            List<ProductImage> productImages = new List<ProductImage>();
            foreach (IFormFile Photo in product.Photos)
            {
                if (!Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", " sekil elave edin");
                    return View();
                }
                if (Photo.IsOlder1MB())
                {
                    ModelState.AddModelError("Photo", " 1 mb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images", "product");
                ProductImage productImage = new ProductImage
                {
                    Image = await Photo.SaveFileAsync(folder),

                };
                productImages.Add(productImage);

            };
            List<ProductCategory> productCategories = new List<ProductCategory>();

            ProductCategory mainProductCategory = new ProductCategory
            {
                CategoryId = (int)mainCatId,
            
            };

            productCategories.Add(mainProductCategory);

            if(childCatId != null)
            {
                ProductCategory childProductCategory = new ProductCategory
                {
                    CategoryId = (int)childCatId,
                };
                productCategories.Add(childProductCategory);
            }
            
            product.ProductCategories = productCategories;
            product.ProductImages = productImages;
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProductImage(int? proImgId)
        {
            ProductImage? productImage = await _db.ProductImages.Include(x=>x.Product).ThenInclude(x=>x.ProductImages).FirstOrDefaultAsync(x => x.Id == proImgId);
            int productImagesCount =productImage.Product.ProductImages.Count;
            _db.ProductImages.Remove(productImage);
            if (productImagesCount == 2)
            {
                return Content("stop");
            }
            await _db.SaveChangesAsync();
            
            return Content("Ok");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product? product = await _db.Products.Include(x=>x.ProductCategories).ThenInclude(x=>x.Category).ThenInclude(x=>x.Children).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id==id);
            if (product == null)
            {
                return BadRequest();
            }
            ViewBag.MainCategories = await _db.Categories.Where(x => x.IsMain).ToListAsync();


            return View(product);
        }
           
    }
}

