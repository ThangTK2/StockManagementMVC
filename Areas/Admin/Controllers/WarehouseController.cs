using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagementMVC.Models;
using StockManagementMVC.Repository;
using X.PagedList.Extensions;

namespace StockManagementMVC.Areas.Admin.Controllers
{
    [Authorize]      // chỉ cho phép user đã đăng nhập
    [Area("Admin")]  //Dùng để tổ chức dự án theo module (Areas).
    public class WarehouseController : Controller
    {
        private readonly DataContext _context;  //DataContext : trong folder Repository
        public WarehouseController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        // GET: Import
        [HttpGet]
        public IActionResult Import()
        {
            ViewBag.Products = _context.Products
            .Select(p => new
            {
                p.Id,
                NameWithQuantity = p.ProductName
                    + " (Tồn: " + p.ProductQuantity + " " + p.ProductUnit + ")"
            }).ToList();
            return View();
        }

        // POST: Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(WarehouseTransactionModel model)
        {
            ModelState.Remove("TransactionType");
            ModelState.Remove("Product");
            if (ModelState.IsValid)   //ModelState là một thuộc tính có sẵn của lớp Controller trong ASP.NET Core MVC
            {
                // Lấy sản phẩm theo Id
                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", "Sản phẩm không tồn tại.");
                    ViewBag.Products = _context.Products.ToList();
                    return View(model);
                }

                // Cập nhật số lượng trong kho
                product.ProductQuantity += model.Quantity;

                // Lưu giao dịch nhập kho
                model.TransactionDate = DateTime.Now;
                model.TransactionType = "Nhập kho";

                _context.WarehouseTransactions.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Nhập kho thành công!";
                return RedirectToAction("Import");
            }

            ViewBag.Products = _context.Products.ToList();
            return View(model);
        }


        // GET: Export
        [HttpGet]
        public IActionResult Export()
        {
            ViewBag.Products = _context.Products
                .Select(p => new
                {
                    p.Id,
                    NameWithQuantity = p.ProductName
                        + " (Tồn: " + p.ProductQuantity + " " + p.ProductUnit + ")"
                }).ToList();

            return View();
        }

        // POST: Export
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(WarehouseTransactionModel model)
        {
            ModelState.Remove("TransactionType");
            ModelState.Remove("Product");

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", "Sản phẩm không tồn tại.");
                    ViewBag.Products = _context.Products.ToList();
                    return View(model);
                }

                // Kiểm tra tồn kho
                if (product.ProductQuantity < model.Quantity)
                {
                    ModelState.AddModelError("", "Số lượng xuất vượt quá tồn kho!");
                    ViewBag.Products = _context.Products
                        .Select(p => new
                        {
                            p.Id,
                            NameWithQuantity = p.ProductName
                                + " (Tồn: " + p.ProductQuantity + " " + p.ProductUnit + ")"
                        }).ToList();
                    return View(model);
                }

                // Cập nhật số lượng (trừ đi khi xuất)
                product.ProductQuantity -= model.Quantity;

                // Ghi lại giao dịch xuất kho
                model.TransactionDate = DateTime.Now;
                model.TransactionType = "Xuất kho";

                _context.WarehouseTransactions.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xuất kho thành công!";
                return RedirectToAction("Export");
            }

            ViewBag.Products = _context.Products.ToList();
            return View(model);
        }

        // Danh sách nhập/xuất kho
        public IActionResult ListWarehouse(int? page, string filter)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.WarehouseTransactions
                .Include(t => t.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                // chuẩn hoá tham số filter (client gửi "Nhap" hoặc "Xuat")
                var f = filter.Trim().ToLowerInvariant();

                if (f == "nhap" || f == "import")
                {
                    query = query.Where(t =>
                        t.TransactionType != null &&
                        (
                            t.TransactionType.ToLower().Contains("nhập") ||
                            t.TransactionType.ToLower().Contains("nhap") ||
                            t.TransactionType.ToLower().Contains("import")
                        ));
                }
                else if (f == "xuat" || f == "export")
                {
                    query = query.Where(t =>
                        t.TransactionType != null &&
                        (
                            t.TransactionType.ToLower().Contains("xuất") ||
                            t.TransactionType.ToLower().Contains("xuat") ||
                            t.TransactionType.ToLower().Contains("export")
                        ));
                }
            }

            var transactions = query
                .OrderByDescending(t => t.TransactionDate)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentFilter = filter;
            return View(transactions);
        }

    }
}
