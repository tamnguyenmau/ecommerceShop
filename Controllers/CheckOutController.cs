using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceShop.Extension;
using ECommerceShop.Models;
using ECommerceShop.Req;
using ECommerceShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShop.Controllers
{
    public class CheckOutController : Controller
    {
        

        private readonly DBContext _context;
        public INotyfService _notifyService { get; }

        public CheckOutController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
    

        }
        [HttpGet]
        [Route("thanh-toan", Name = "Checkout")]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            CheckOut model = new CheckOut();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.Fullname = khachhang.FullName;
                model.Phone = khachhang.Phone;
                model.Email = khachhang.Email;
                model.Address = khachhang.Address;
            }

            ViewBag.GioHang = cart;
            return View(model);
        }
        [HttpPost]
        [Route("thanh-toan", Name = "Checkout")]
        public IActionResult Index(CheckOut muaHang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            CheckOut model = new CheckOut();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.Fullname = khachhang.FullName;
                model.Phone = khachhang.Phone;
                model.Email = khachhang.Email;
                model.Address = khachhang.Address;
                khachhang.Address = muaHang.Address;

                _context.Update(khachhang);
                _context.SaveChanges();
            }
            Order donhang = new Order();
            donhang.CustomerId = model.CustomerId;
            donhang.Address = model.Address;
            donhang.OrderDate = DateTime.Now;
            donhang.TransactStatusId = 1;
            donhang.Paid = false;
            donhang.Status = true;
            donhang.Note = model.Note;
            donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
            _context.Add(donhang);
            _context.SaveChanges();

            foreach (var item in cart)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderId = donhang.OrderId;
                orderDetail.ProductId = item.product.ProductId;
                orderDetail.Amount = item.amount;
                orderDetail.Total = donhang.TotalMoney;
                orderDetail.Price = item.product.Price;
                orderDetail.CreateDate = DateTime.Now;
                _context.Add(orderDetail);
            }
            _context.SaveChanges();
            HttpContext.Session.Remove("GioHang");
            _notifyService.Success("Đặt hàng thành công");
            return RedirectToAction("Success");
        }
        [Route("dat-hang-thanh-cong", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = "/dat-hang-thanh-cong" });
                }
                var khachhang = _context.Customers.AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders
                    .Where(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                BuySuccess success = new BuySuccess();
                success.FullName = khachhang.FullName;
                success.Phone = khachhang.Phone;
                success.Address = khachhang.Address;
                ViewBag.GioHang = cart;
                return View(success);
            }
            catch
            {
                return View();
            }
        
        }

        
    }
}
