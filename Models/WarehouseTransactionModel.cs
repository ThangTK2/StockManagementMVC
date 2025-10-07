using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagementMVC.Models
{
    public class WarehouseTransactionModel
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public ProductModel Product { get; set; }  //bang db Product

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [StringLength(50)]
        public string TransactionType { get; set; } // "Import" hoặc "Export"

        [Required(ErrorMessage = "Vui lòng nhập ghi chú.")]
        [StringLength(200)]
        public string Notes { get; set; }
    }
}
