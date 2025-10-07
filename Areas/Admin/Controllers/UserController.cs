using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockManagementMVC.Models.ViewModel;
using StockManagementMVC.Repository;

namespace StockManagementMVC.Areas.Admin.Controllers
{
    [Authorize]      // chỉ cho phép user đã đăng nhập
    [Area("Admin")]  //Dùng để tổ chức dự án theo module (Areas).
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;  //IdentityUser lớp người dùng mà bạn định nghĩa để kế thừa từ IdentityUser trong ASP.NET || _userManager: tến biến
        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Admin/User
        public IActionResult Index()
        {
            // Lấy danh sách người dùng
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Create người dùng
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModal model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Tạo người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    if (error.Code != null && error.Code.ToLower().Contains("password"))
                    {
                        ModelState.AddModelError(nameof(model.Password), error.Description);
                    }
                    else if (error.Code != null && (error.Code.ToLower().Contains("email") || error.Code.ToLower().Contains("user")))
                    {
                        ModelState.AddModelError(nameof(model.Email), error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            TempData["ErrorMessage"] = "Tạo người dùng thất bại. Vui lòng thử lại.";
            return View(model);
        }


        // Edit người dùng
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UserViewModal
            {
                Email = user.Email,
                Password = "" // Không hiển thị mật khẩu cũ
            };
            ViewData["UserId"] = id; // lưu id để POST
            return View(model);
        }
        // Edit người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModal model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Nếu có nhập mật khẩu mới
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }
                }

                TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        //Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Xóa người dùng thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Xóa người dùng thất bại!";
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại!";
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
