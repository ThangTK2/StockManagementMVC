using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockManagementMVC.Models.ViewModel;

namespace StockManagementMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;  //IdentityUser lớp người dùng mà bạn định nghĩa để kế thừa từ IdentityUser trong ASP.NET
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()  //tên phải đặt giống view nó mới hiểu trả về view
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Admin");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
            }
            else
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser //IdentityUser lớp người dùng mà bạn định nghĩa để kế thừa từ IdentityUser trong ASP.NET
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Thêm thông báo vào TempData
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    // Đăng nhập luôn sau khi đăng ký
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account"); // ("page", "controller")
                }

                // Nếu có lỗi thì add vào ModelState để hiển thị
                foreach (var error in result.Errors)
                {
                    // Nếu lỗi liên quan mật khẩu (Identity thường có code bắt đầu bằng "Password" hoặc "PasswordTooShort", ...)
                    if (error.Code != null && error.Code.ToLower().StartsWith("password"))
                    {
                        // Gán vào field Password -> sẽ hiển thị dưới <span asp-validation-for="Password">
                        ModelState.AddModelError(nameof(model.Password), error.Description);
                    }
                    // Nếu lỗi liên quan email/username (ví dụ duplicate email)
                    else if (error.Code != null && (error.Code.ToLower().Contains("email") || error.Code.ToLower().Contains("user")))
                    {
                        ModelState.AddModelError(nameof(model.Email), error.Description);
                    }
                    else
                    {
                        // Các lỗi chung khác -> hiển thị ở validation summary
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            TempData["ErrorMessage"] = "Đăng ký thất bại. Vui lòng thử lại.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Login", "Account");
        }

    }
}
