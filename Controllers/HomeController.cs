using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceShop.Models;
using ECommerceShop.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerceShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBContext _context;

        public INotyfService _notifyService { get; }

        public HomeController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        public IActionResult Index()
        {
            HomeView model = new HomeView();
            var lsProducts = _context.Products.AsNoTracking()
                .Where(x=>x.Status == true && x.HomeFlag == true)
                .OrderByDescending(x=>x.DateCreated)
                .ToList();
            List<ProductHome> lsProductsView = new List<ProductHome>();
            var lsCats = _context.Categories
                .AsNoTracking()
                .Where(x => x.Status == true)
                .OrderByDescending(x => x.Position)
                .ToList();
            foreach (var item in lsCats)
            {
                ProductHome productHome = new ProductHome();
                productHome.category = item;
                productHome.lsProducts = lsProducts.Where(x=>x.CatId == item.CatId).ToList();
                lsProductsView.Add(productHome);
            }
            model.Products = lsProductsView;
            ViewBag.AllProducts = lsProducts;
            return View(model);
        }
        [Route("ve-chung-toi")]
        public IActionResult About()
        {
            return View();
        }
        [Route("lien-he")]
        public IActionResult Contact()
        {
            return View();
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