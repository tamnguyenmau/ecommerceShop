using System.ComponentModel.DataAnnotations;

namespace ECommerceShop.Areas.Admin.Models
{
    public class EditProfile
    {
        [Key]
        public int AccountId { get; set; }

        public string? FullName { get; set; }
        public string? Phone {  get; set; }
        public string? Avatar { get; set; }
    }
}
