﻿using System.ComponentModel.DataAnnotations;

namespace ECommerceShop.Req
{
    public class ChangePassword
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 6 kí tự")]
        public string NewPassword { get; set; }

        [MinLength(6, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 6 kí tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("CurrentPassword", ErrorMessage = "Mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }
    }
}