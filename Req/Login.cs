using System.ComponentModel.DataAnnotations;

namespace ECommerceShop.Req
{
    public class Login
    {
        [Key]
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
