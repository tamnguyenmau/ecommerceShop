using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceShop.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;
using ECommerceShop.Helper;

namespace ECommerceShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly DBContext _context;
        public INotyfService _notifyService { get; }
        public AdminProductsController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        // GET: Admin/AdminProducts
        public async Task<IActionResult> Index(int? page, int CatID = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

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
        public IActionResult Filter(int CatID = 0)
        {

            var url = $"/Admin/AdminProducts?CatId={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminProducts";
            }
            return Json(new { status = "success", RedirectUrl = url });
        }
        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Content,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,Bestsellers,HomeFlag,Status,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Product product, Microsoft.AspNetCore.Http.IFormFile thumb)
        {
            product.ProductName = Utilities.ToTitleCase(product.ProductName);
            if (thumb != null)
            {
                string extension = Path.GetExtension(thumb.FileName);
                string image = Utilities.SEOUrl(product.ProductName) + extension;
                product.Thumb = await Utilities.UploadFile(thumb, @"products", image.ToLower());
            }

            if (string.IsNullOrEmpty(product.Thumb))
            {
                product.Thumb = "default.jpg";
            }
            if (product.Price == null)
            {
                product.Price = 0;
            }
            if (product.UnitslnStock == null)
            {
                product.UnitslnStock = 0;
            }

            product.Alias = Utilities.SEOUrl(product.ProductName);
            product.DateCreated = DateTime.Now;
            _context.Add(product);
            await _context.SaveChangesAsync();
            _notifyService.Success("Tạo mới thành công", 2);
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Content,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,Bestsellers,HomeFlag,Status,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Product product, Microsoft.AspNetCore.Http.IFormFile thumb)
        {
            if (id != product.ProductId)
            {
                _notifyService.Error("Không tìm thấy Id", 2);

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = Utilities.ToTitleCase(product.ProductName);
                    if (thumb != null)
                    {
                        string extension = Path.GetExtension(thumb.FileName);
                        string image = Utilities.SEOUrl(product.ProductName) + extension;
                        product.Thumb = await Utilities.UploadFile(thumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.Thumb))
                    {

                        product.Thumb = "default.jpg";
                    }

                    product.Alias = Utilities.SEOUrl(product.ProductName);
                    product.DateModified = DateTime.Now;
                    _notifyService.Success("Cập nhật thành công", 2);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        _notifyService.Error("Không tìm thấy sản phẩm", 2);

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                _notifyService.Error("Không tìm thấy Id", 2);

                return Problem("Entity set 'DBContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _notifyService.Success("Xoá thành công", 2);

                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
