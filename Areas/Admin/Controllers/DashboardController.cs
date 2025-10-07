using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockManagementMVC.Areas.Admin.Controllers
{
    [Authorize]      // chỉ cho phép user đã đăng nhập
    [Area("Admin")]  //Dùng để tổ chức dự án theo module (Areas).
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
