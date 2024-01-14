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
    public class AdminArticlesController : Controller
    {
        private readonly DBContext _context;

        public INotyfService _notifyService { get; }


        public AdminArticlesController(DBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        // GET: Admin/AdminArticles
        public async Task<IActionResult> Index(int? page, int CatID = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            List<Article> IsArticle = new List<Article>();
            if (CatID != 0)
            {
                IsArticle = _context.Articles
                .AsNoTracking()
                .Where(c => c.CatId == CatID)
                .Include(c => c.Cat)
                .OrderBy(x => x.ArticleId)
                .ToList();
            }
            else
            {
                IsArticle = _context.Articles
                .AsNoTracking()
                .Include(c => c.Cat)
                .OrderBy(x => x.ArticleId)
                .ToList();
            }

            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);
            PagedList<Article> models = new PagedList<Article>(IsArticle.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminArticles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Account)
                .Include(a => a.Cat)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Admin/AdminArticles/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId");
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId");
            return View();
        }

        // POST: Admin/AdminArticles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,Title,Description,Content,Thumb,Status,CreateDate,Author,AccountId,Tags,CatId,IsHost,IsNewFeed,MetaDesc,MetaKey,Views,Alias")] Article article, Microsoft.AspNetCore.Http.IFormFile thumb)
        {
            if (ModelState.IsValid)
            {
                article.Title = Utilities.ToTitleCase(article.Title);
                if (thumb != null)
                {
                    string extension = Path.GetExtension(thumb.FileName);
                    string image = Utilities.SEOUrl(article.Title) + extension;
                    article.Thumb = await Utilities.UploadFile(thumb, @"news", image.ToLower());
                }
                if (string.IsNullOrEmpty(article.Thumb))
                {
                    article.Thumb = "default.jpg";
                }
                article.CreateDate = DateTime.Now;
                article.Alias = Utilities.SEOUrl(article.Title);
                _context.Add(article);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm mới thành công", 2);
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", article.AccountId);
            ViewData["category"] = new SelectList(_context.Categories, "CatId", "CatName", article.CatId);

            return View(article);
        }

        // GET: Admin/AdminArticles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", article.AccountId);
            ViewData["category"] = new SelectList(_context.Categories, "CatId", "CatName", article.CatId);
            return View(article);
        }

        // POST: Admin/AdminArticles/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,Title,Description,Content,Thumb,Status,CreateDate,Author,AccountId,Tags,CatId,IsHost,IsNewFeed,MetaDesc,MetaKey,Views,Alias")] Article article, Microsoft.AspNetCore.Http.IFormFile thumb)
        {
            if (id != article.ArticleId)
            {
                _notifyService.Error("Không tìm thấy Id", 2);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    article.Title = Utilities.ToTitleCase(article.Title);
                    if (thumb != null)
                    {
                        string extension = Path.GetExtension(thumb.FileName);
                        string image = Utilities.SEOUrl(article.Title) + extension;
                        article.Thumb = await Utilities.UploadFile(thumb, @"news", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(article.Thumb))
                    {
                        article.Thumb = "default.jpg";
                    }
                    article.CreateDate = DateTime.Now;
                    article.Alias = Utilities.SEOUrl(article.Title);
                    _notifyService.Success("Cập nhật thành công", 2);
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        _notifyService.Error("Không tìm thấy bài viết", 2);

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", article.AccountId);
            ViewData["category"] = new SelectList(_context.Categories, "CatId", "CatName", article.CatId);
            return View(article);
        }

        // GET: Admin/AdminArticles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Account)
                .Include(a => a.Cat)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Admin/AdminArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Articles == null)
            {
                _notifyService.Error("Không tìm thấy Id", 2);

                return Problem("Entity set 'DBContext.Articles'  is null.");
            }
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _notifyService.Success("Xoá thành công", 2);

                _context.Articles.Remove(article);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
          return (_context.Articles?.Any(e => e.ArticleId == id)).GetValueOrDefault();
        }
    }
}
