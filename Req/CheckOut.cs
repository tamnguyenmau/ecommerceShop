using System.ComponentModel.DataAnnotations;

namespace ECommerceShop.Req
{
    public class CheckOut
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Fullname { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }
        public int PaymentID { get; set; }
        public string Note { get; set; }
    }
}
