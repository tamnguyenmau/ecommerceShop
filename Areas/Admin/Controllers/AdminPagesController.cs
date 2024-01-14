﻿using System;
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
    public class AdminPagesController : Controller
    {
        private readonly DBContext _context;

        public INotyfService _notifyService { get; }

        public AdminPagesController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        // GET: Admin/AdminPages
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            var IsPages = _context.Pages
                .AsNoTracking()
                .OrderBy(x => x.PageId);

            PagedList<Page> models = new PagedList<Page>(IsPages, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminPages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Admin/AdminPages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminPages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageId,PageName,Content,Thumb,Status,Title,MetaDesc,MetaKey,Alias,CreateTime,Position")] Page page, Microsoft.AspNetCore.Http.IFormFile thumb)
        {
            if (ModelState.IsValid)
            {
                if (thumb != null)
                {
                    string extension = Path.GetExtension(thumb.FileName);
                    string imageName = Utilities.SEOUrl(page.PageName) + extension;
                    page.Thumb = await Utilities.UploadFile(thumb, @"pages", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(page.Thumb))
                {
                    page.Thumb = "default.jpg";
                }
                page.Alias = Utilities.SEOUrl(page.PageName);
                page.CreateTime = DateTime.Now;
                _context.Add(page);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm mới thành công", 2);
                return RedirectToAction(nameof(Index));
            }
            return View(page);
        }

        // GET: Admin/AdminPages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        // POST: Admin/AdminPages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PageId,PageName,Content,Thumb,Status,Title,MetaDesc,MetaKey,Alias,CreateTime,Position")] Page page, Microsoft.AspNetCore.Http.IFormFile thumb)
        {
            if (id != page.PageId)
            {
                _notifyService.Error("Không tìm thấy id", 2);

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (thumb != null)
                    {
                        string extension = Path.GetExtension(thumb.FileName);
                        string imageName = Utilities.SEOUrl(page.PageName) + extension;
                        page.Thumb = await Utilities.UploadFile(thumb, @"pages", imageName.ToLower());
                    }
                    if (string.IsNullOrEmpty(page.Thumb))
                    {
                        page.Thumb = "default.jpg";
                    }
                    page.Alias = Utilities.SEOUrl(page.PageName);
                    _context.Update(page);
                    _notifyService.Success("Sửa thành công", 2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.PageId))
                    {
                        _notifyService.Error("Page không tồn tại", 2);

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(page);
        }

        // GET: Admin/AdminPages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/AdminPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pages == null)
            {
                return Problem("Entity set 'DBContext.Pages'  is null.");
            }
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                _context.Pages.Remove(page);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
          return (_context.Pages?.Any(e => e.PageId == id)).GetValueOrDefault();
        }
    }
}