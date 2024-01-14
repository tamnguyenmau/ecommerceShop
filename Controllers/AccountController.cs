using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceShop.Extension;
using ECommerceShop.Helper;
using ECommerceShop.Models;
using ECommerceShop.Req;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext _context;

        public INotyfService _notifyService { get; }

        public AccountController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }
        [Authorize]
        [HttpGet]
        [AllowAnonymous]
        [Route("thong-tin-ca-nhan", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    var lsdonhang = _context.Orders.AsNoTracking()
                        .Include(x=>x.TransactStatus)
                        .Where(x => x.CustomerId == khachhang.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.DonHang = lsdonhang;
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("doi-mat-khau", Name = "ChangePassword")]
        public IActionResult ChangePassword(ChangePassword model)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID == null)
            {
                _notifyService.Error("Tài khoản không tồn tại", 2);
                return RedirectToAction("Login", "Account");
            }
            var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanID));
            if (taikhoan == null)
            {
                _notifyService.Error("Không tìm thấy tài khoản", 2);
                return View();
            }
            var pass = (model.CurrentPassword.Trim() + taikhoan.Salt.Trim()).ToMD5();
            if (pass == taikhoan.Password)
            {
                string passnew = (model.NewPassword.Trim() + taikhoan.Salt.Trim()).ToMD5();
                taikhoan.Password = passnew;
                _context.Update(taikhoan);
                _context.SaveChanges();
                _notifyService.Success("Đổi mật khẩu thành công", 2);
                return RedirectToAction("Dashboard", "Account");
            }
            return RedirectToAction("Dashboard", "Account");
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky", Name = "Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky", Name = "Register")]
        public async Task<IActionResult> Register(Register register)
        {
            var check = _context.Customers.FirstOrDefault(x => x.Email == register.Email);
            if (check == null)
            {
                string salt = Utilities.GetRamdomKey();
                Customer khachhang = new Customer
                {
                    FullName = register.FullName,
                    BirthDay = register.BirthDay,
                    Address = register.Address,
                    Email = register.Email,
                    Phone = register.Phone,
                    Status = true,
                    Salt = salt,
                    CreateTime = DateTime.Now,
                    Password = (register.Password + salt.Trim()).ToMD5()
                };
                bool isEmail = Utilities.IsValidEmail(register.Email);
                if (!isEmail)
                {
                    return View(register);
                }
                _context.Add(khachhang);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,khachhang.FullName),
                            new Claim("CustomerId", khachhang.CustomerId.ToString())
                        };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
                _notifyService.Success("Đăng ký thành công", 2);
                return Redirect("thong-tin-ca-nhan");
            }
            else
            {
                ViewBag.Error = "Email đã tồn tại";
                return View();
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap", Name = "Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap", Name = "Login")]
        public async Task<IActionResult> Login(Login customer, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.Email);
            if (khachhang == null)
            {
                return Redirect("dang-ky");
            }
            string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
            if (khachhang.Password != pass)
            {
                _notifyService.Error("Sai mật khẩu", 2);

                ViewBag.Error = "Sai thông tin đăng nhập";
                return View(customer);
            }
            if (khachhang.Status == false)
            {
                _notifyService.Error("Đăng nhập không thành công", 2);
            }
            HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());

            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,khachhang.FullName),
                        new Claim("CustomerId",khachhang.CustomerId.ToString())
                    };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
            
            _notifyService.Success("Đăng nhập thành công", 2);
            return Redirect("thong-tin-ca-nhan");
        }
        [HttpGet]
        [Route("dang-xuat", Name = "Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return Redirect("dang-nhap");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                {
                    return Json(data: "Số điện thoại: " + Phone + "Đã được sử dụng");

                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                {
                    return Json(data: "Email" + Email + "Đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }
    }
}
