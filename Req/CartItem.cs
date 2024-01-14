using ECommerceShop.Models;

namespace ECommerceShop.Req
{
    public class CartItem
    {
        public Product? product { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * product.Price.Value;
    }
}
