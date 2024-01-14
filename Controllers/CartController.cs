using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceShop.Models;
using ECommerceShop.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShop.Controllers
{
    public class CartController : Controller
    {
        private readonly DBContext _context;
        public INotyfService _notifyService { get; }

        public CartController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        [HttpPost]
        [Route("api/detailcart")]
        public async Task<IActionResult> Detail(int? id)
        {
            
            var taikhoanID = HttpContext.Session.GetString("CustomerId");

            var chitietdonhang = _context.OrderDetails
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(x => x.OrderId == id)
                .OrderBy(x => x.OrderDetailId)
                .ToList();
            ViewBag.Chitiet = chitietdonhang;

            return PartialView("DetailCart", chitietdonhang);
        }
    }
}
