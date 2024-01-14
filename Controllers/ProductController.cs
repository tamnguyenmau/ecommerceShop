using ECommerceShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace ECommerceShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly DBContext _context;
        public ProductController(DBContext dbContext)
        {
            _context = dbContext;
        }
        [Route("san-pham", Name = "Product")]
        public async Task<IActionResult> Index(int? page, int CatID = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 12;

            List<Product> IsProducts = new List<Product>();
            if (CatID != 0)
            {
                IsProducts = _context.Products
                   .AsNoTracking()
                   .Where(c => c.CatId == CatID)
                   .Include(c => c.Cat)
                   .OrderBy(x => x.ProductId)
                   .ToList();
            }
            else
            {
                IsProducts = _context.Products
               .AsNoTracking()
               .Include(c => c.Cat)
               .OrderBy(x => x.ProductId)
               .ToList();
            }
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);
            PagedList<Product> models = new PagedList<Product>(IsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        [Route("/{Alias}", Name = "ListProduct")]
        public IActionResult List(string Alias, int page = 1)
        {
            try
            {
                var pageSize = 8;
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == danhmuc.CatId)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProducts, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //filter
        public IActionResult Filter(int CatID = 0)
        {

            var url = $"/san-pham?CatId={CatID}";
            if (CatID == 0)
            {
                url = $"/san-pham";
            }
            return Json(new { status = "success", RedirectUrl = url });
        }

        [Route("/{Alias}-{id}.html", Name = "ProductDetail")]
        public IActionResult Detail(int id)
        {
            var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            var lsProduct = _context.Products.AsNoTracking()
                .Where(x => x.CatId == product.CatId && x.ProductId != id && x.Status == true)
                .Take(4)
                .OrderBy(x => x.DateCreated)
                .ToList();
            ViewBag.SanPham = lsProduct;
            return View(product);
        }
    }
}
