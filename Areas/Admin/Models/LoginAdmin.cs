using System.ComponentModel.DataAnnotations;

namespace ECommerceShop.Areas.Admin.Models
{
    public class LoginAdmin
    {
        [Key]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required(ErrorMessage = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string? Password { get; set; }
    }
}
