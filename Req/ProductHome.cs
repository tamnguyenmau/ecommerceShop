using ECommerceShop.Models;

namespace ECommerceShop.Req
{
    public class ProductHome
    {
        public Category category { get; set; }
        public List<Product> lsProducts { get; set;}
    }
}
