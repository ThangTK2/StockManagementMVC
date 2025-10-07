using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockManagementMVC.Models;

namespace StockManagementMVC.Repository
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<ProductModel> Products { get; set; }
        //DbSet<ProductModel> Products: Đại diện cho bảng Products trong SQL Server.(CRUD)

        public DbSet<WarehouseTransactionModel> WarehouseTransactions { get; set; }
    }
}
