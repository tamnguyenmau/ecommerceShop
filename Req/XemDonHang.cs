using ECommerceShop.Models;

namespace ECommerceShop.Req
{
    public class XemDonHang
    {
        public Order DonHang { get; set; }

        public List<OrderDetail> ChiTietDonHang { get; set; }

    }
}
