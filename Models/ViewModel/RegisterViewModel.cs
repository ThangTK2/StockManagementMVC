using System.ComponentModel.DataAnnotations;

namespace StockManagementMVC.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
