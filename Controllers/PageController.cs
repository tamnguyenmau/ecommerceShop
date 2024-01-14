using ECommerceShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShop.Controllers
{
    public class PageController : Controller
    {
        private readonly DBContext _context;
        public PageController(DBContext context)
        {
            _context = context;
        }
        [Route("trang/{Alias}", Name = "PageDetails")]
        public IActionResult Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return RedirectToAction("Index", "Home");
            }
            var page = _context.Pages.AsNoTracking().SingleOrDefault(x=>x.Alias == Alias);
            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(page);
        }
    }
}
