using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CNPM_DeMo.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Principal;

namespace CNPM_DeMo.Controllers
{
    public class AuthController : Controller
    {
        private readonly LibManageDbContext _context;

        public AuthController(LibManageDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. Tìm tài khoản theo Tên đăng nhập và Mật khẩu trước
            var acc = await _context.Accounts
                .FirstOrDefaultAsync(a => a.TenDangNhap == username && a.MatKhauMaHoa == password);

            if (acc == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }

            // 2. Nếu tìm thấy tài khoản nhưng trạng thái đang bị khóa (IsActive = false)
            if (!acc.IsActive)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị vô hiệu hóa hoặc tạm khóa. Vui lòng liên hệ Admin!";
                return View();
            }

            // 3. Đăng nhập thành công, nạp thông tin quyền hạn vào Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, acc.TenDangNhap),
                new Claim("FullName", acc.HoTen),
                // Ép kiểu Enum VaiTro sang String (Ví dụ: "Admin" hoặc "ThuThu") để hệ thống nhận diện
                new Claim(ClaimTypes.Role, acc.VaiTro.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            // Xóa sạch Cookie định danh của người dùng khỏi trình duyệt
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Điều hướng ngay lập tức về trang hiển thị form Login
            return RedirectToAction("Login", "Auth");
        }

        // GIAO DIỆN BÁO LỖI KHI TRUY CẬP SAI QUYỀN HẠN
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}