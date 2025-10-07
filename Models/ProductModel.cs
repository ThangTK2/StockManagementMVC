using System.ComponentModel.DataAnnotations;

namespace StockManagementMVC.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        [StringLength(100, ErrorMessage = "Mã sản phẩm tối đa 100 ký tự")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Mô tả tối đa 200 ký tự")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số dương")]
        public int ProductQuantity { get; set; }

        [Required(ErrorMessage = "Đơn vị không được để trống")]
        [StringLength(100, ErrorMessage = "Đơn vị tối đa 100 ký tự")]
        public string ProductUnit { get; set; }

        [Required(ErrorMessage = "Vị trí kho không được để trống")]
        public string Location { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}
