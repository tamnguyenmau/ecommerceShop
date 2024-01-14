using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly DBContext _context;
        public INotyfService _notifyService { get; }
        public SearchController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        //Get: Search/FindProduct
        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> products = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            products = _context.Products
                .AsNoTracking()
                .Include(c => c.Cat)
                .Where(x => x.ProductName.Contains(keyword))
                .OrderBy(x => x.ProductName)
                .Take(10)
                .ToList();
            if (products == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
                return PartialView("ListProductsSearchPartial", products);

        }
    }
}
