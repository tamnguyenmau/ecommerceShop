using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceShop.Models;
using ECommerceShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceShop.Controllers
{
    public class VnPayController : Controller
    {
        private readonly IVnPayService _vnPayService;

        private readonly DBContext _context;
        public INotyfService _notifyService { get; }

        public VnPayController(DBContext context, INotyfService notifyService, IVnPayService vnPayService)
        {
            _context = context;
            _notifyService = notifyService;
            _vnPayService = vnPayService;

        }
        [Route("vnpay")]

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        [Route("vnpay/data")]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }

    }
}
