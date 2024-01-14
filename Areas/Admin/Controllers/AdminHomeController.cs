    using Microsoft.AspNetCore.Mvc;

namespace ECommerceShop.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        [Area("Admin")]
        [Route("admin", Name = "AdminHome")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AccountId") != null)
            {
                
            return View();
            }
            else
            {
                return RedirectToAction("Login","AdminAccounts");
            }
        }
    }
}
