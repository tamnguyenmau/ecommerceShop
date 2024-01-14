using System.ComponentModel.DataAnnotations;

namespace ECommerceShop.Req
{
    public class Update
    {
        [Key]
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDay { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
