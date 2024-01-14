﻿using ECommerceShop.Extension;
using ECommerceShop.Req;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceShop.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}
