using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceShop.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceShop.Helper;
using ECommerceShop.Req;
using ECommerceShop.Extension;
using ECommerceShop.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerceShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly DBContext _context;

        public INotyfService _notifyService { get; }
 

        public AdminAccountsController(DBContext context, INotyfService notifyService)
        {
            _context = context;
 
            _notifyService = notifyService;
        }

       

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index()
        {
            //filter
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "Description");
            List<SelectListItem> isStatus = new List<SelectListItem>();
            isStatus.Add(new SelectListItem()
            {
                Text = "Active",
                Value = "1"
            });
            isStatus.Add(new SelectListItem()
            {
                Text = "Block",
                Value = "0"
            });

            ViewData["isStatus"] = isStatus;
            var dBContext = _context.Accounts.Include(a => a.Role);
            return View(await dBContext.ToListAsync());
        }
        [Authorize]
        [HttpGet]
        [AllowAnonymous]
     
        public IActionResult Profile()
        {

            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Accounts.Include(x => x.Role).AsNoTracking()
                    .SingleOrDefault(x => x.AccountId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }
        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                _notifyService.Error("Không tìm thấy Id", 2);

                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                _notifyService.Error("Không tìm thấy tài khoản", 2);

                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Phone,Email,Password,Status,FullName,RoleId,CreateTime,Avatar")] Account account)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Accounts.FirstOrDefault(x=>x.Email == account.Email);
                if (check == null)
                {
                    string salt = Utilities.GetRamdomKey();
                    account.Salt = salt;
                    account.CreateTime = DateTime.Now;
                    account.Status = true;
                    account.Avatar = "default.png";
                    account.Password = (account.Phone + salt.Trim()).ToMD5();

                    _context.Add(account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Email đã tồn tại";
                    return View(account);

                }
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            _notifyService.Success("Thêm thành công", 2);

            return View(account);
        }
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
     
        public IActionResult Login(Login admin)
        {
            
            if (admin.Email == null && admin.Password == null)
            {
                return View();

            }
            var quantri = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == admin.Email);
            if (quantri == null)
            {
                ViewBag.LoginMessage = "Không tìm thấy quản trị";
                return View();
            }
            string pass = (admin.Password + quantri.Salt.Trim()).ToMD5();
            if (quantri.Password != pass)
            {
                ViewBag.LoginMessage = "Sai mật khẩu";
                return View();

            }
            if (quantri.Status == false)
            {
                ViewBag.LoginMessage = "Tài khoản bị khoá";

            }
            HttpContext.Session.SetString("AccountId", quantri.AccountId.ToString());
            HttpContext.Session.SetString("Avatar", quantri.Avatar ?? string.Empty);
            HttpContext.Session.SetString("FullName", quantri.FullName ?? string.Empty);


            var taikhoanID = HttpContext.Session.GetString("AccountId");
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,quantri.FullName),
                        new Claim("AccountId",quantri.AccountId.ToString())
                    };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync(claimsPrincipal);

            _notifyService.Success("Đăng nhập thành công", 2);
            return RedirectToAction("Index", "AdminHome");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccountId");
            return RedirectToAction("Login", "AdminAccounts");
        }
       
        public async Task<IActionResult> ChangePassword()
        {

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        
        public async Task<IActionResult> ChangePassword(ChangePasswordAdmin model)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID == null)
            {
                ViewBag.LoginMessage = "Không tìm thấy quản trị";
                return View();

            }
            var taikhoan = _context.Accounts.Find(Convert.ToInt32(taikhoanID));
            if (taikhoan == null)
            {
                ViewBag.LoginMessage = "Không tìm thấy quản trị";

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
                return Redirect("admin");

            }
            return View();

        }
      
        public async Task<IActionResult> EditProfile()
        {

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
       
        public async Task<IActionResult> EditProfile(EditProfile model,  Microsoft.AspNetCore.Http.IFormFile thumb)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID == null)
            {
                ViewBag.LoginMessage = "Không tìm thấy quản trị";
                return View();

            }
            var taikhoan = _context.Accounts.Find(Convert.ToInt32(taikhoanID));
            if (taikhoan == null)
            {
                ViewBag.LoginMessage = "Không tìm thấy quản trị";

                return View();
            }

            taikhoan.Phone = model.Phone;
            taikhoan.FullName = model.FullName;
            string extension = Path.GetExtension(thumb.FileName);
            string image = Utilities.SEOUrl(taikhoan.FullName) + extension;
            taikhoan.Avatar= await Utilities.UploadFile(thumb, @"avatar", image.ToLower());
            _context.Update(taikhoan);
            _context.SaveChanges();
            HttpContext.Session.SetString("Avatar", taikhoan.Avatar ?? string.Empty);
            _notifyService.Success("Đổi mật khẩu thành công", 2);
            return RedirectToAction("Profile");

        }
        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // POST: Admin/AdminAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Phone,Email,Password,Salt,Status,FullName,RoleId,LastLogin,CreateTime")] Account account)
        {
            if (id != account.AccountId)
            {
                _notifyService.Error("Không tìm thấy Id", 2);

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(account.Avatar))
                    {

                        account.Avatar = "default.png";
                    }
                    account.LastLogin = DateTime.Now;

                    _context.Update(account);
                    _notifyService.Success("Sửa thành công", 2);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        _notifyService.Error("Tài khoản không tồn tại", 2);

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accounts == null)
            {
                _notifyService.Error("Không tìm thấy Id", 2);

                return Problem("Entity set 'DBContext.Accounts'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _notifyService.Success("Xoá thành công", 2);

                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }
    }
}
