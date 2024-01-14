using ECommerceShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace ECommerceShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly DBContext _context;
        public BlogController(DBContext context)
        {
            _context = context;
        }
        [Route("tin-tuc", Name = "Blog")]
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 6;
            var IsArticle = _context.Articles
                .AsNoTracking()
                .OrderBy(x => x.ArticleId);
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName");

            PagedList<Article> models = new PagedList<Article>(IsArticle, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        [Route("tin-tuc/{Alias}-{id}.html", Name = "BlogDetails")]
        public IActionResult Detail(int id)
        {
            var article = _context.Articles.Include(x => x.Cat).FirstOrDefault(x => x.ArticleId == id);
            if (article == null)
            {
                return RedirectToAction("Index");
            }
            return View(article);
        }
    }
}
