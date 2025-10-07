using System.ComponentModel.DataAnnotations;

namespace StockManagementMVC.Models.ViewModel
{
    public class UserViewModal
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
