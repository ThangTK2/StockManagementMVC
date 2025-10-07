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
    public class ProductController : Controller
    {
        private readonly DataContext _context;  //DataContext : trong folder Repository
        public ProductController(DataContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm
        public IActionResult Index(int? page)
        {
            int pageSize = 10; // số sản phẩm mỗi trang
            int pageNumber = page ?? 1;
            var products = _context.Products
        .OrderByDescending(p => p.CreateDate)
        .ToPagedList(pageNumber, pageSize);
            return View(products);
        }

        // them sản phẩm
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;

                _context.Products.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sản phẩm đã được thêm thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // edit sản phẩm
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == model.Id);
                if (product == null) return NotFound();

                // cập nhật dữ liệu
                product.ProductCode = model.ProductCode;
                product.ProductName = model.ProductName;
                product.ProductDescription = model.ProductDescription;
                product.ProductQuantity = model.ProductQuantity;
                product.ProductUnit = model.ProductUnit;
                product.Location = model.Location;
                product.UpdateDate = DateTime.Now;

                _context.Update(product);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // xoa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }

    }
}
