using CNPM_DeMo.Data;
using CNPM_DeMo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CNPM_DeMo.Controllers
{
    [Authorize]
    public class ReaderController : Controller
    {
        private readonly LibManageDbContext _context;

        public ReaderController(LibManageDbContext context)
        {
            _context = context;
        }

        // 1. CHỨC NĂNG DANH SÁCH ĐỘC GIẢ
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.ReaderCards.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(r => r.HoTen.Contains(searchString) || r.MaThe.Contains(searchString));
            }

            var readers = await query.OrderByDescending(r => r.Id).ToListAsync();
            return View(readers);
        }

        // 2. CHỨC NĂNG TẠO THẺ MỚI (GET: Hiển thị form) - ĐÃ CẬP NHẬT ASYNC
        public async Task<IActionResult> Create()
        {
            // Lấy thời hạn thẻ từ Cấu hình hệ thống (hoặc mặc định là 6)
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            int cardMonths = setting?.ThoiHanTheThang ?? 6;

            // Tự động sinh Mã thẻ và Ngày hết hạn cộng theo cấu hình động
            var newCard = new ReaderCard
            {
                MaThe = "LIB" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                NgayCap = DateTime.Now,
                NgayHetHan = DateTime.Now.AddMonths(cardMonths) // Đã đổi sang biến động
            };

            return View(newCard);
        }

        // 3. CHỨC NĂNG TẠO THẺ MỚI (POST: Xử lý lưu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReaderCard reader)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            int minAge = setting?.DoTuoiToiThieu ?? 18;
            int maxAge = setting?.DoTuoiToiDa ?? 60;
            int cardMonths = setting?.ThoiHanTheThang ?? 6;

            // Bỏ qua kiểm tra danh sách phiếu mượn
            ModelState.Remove("BorrowTickets");

            // Logic kiểm tra độ tuổi bằng biến động
            if (reader.NgaySinh.Year < 1900)
            {
                ModelState.AddModelError("NgaySinh", "Vui lòng nhập năm sinh hợp lệ.");
            }
            else
            {
                var age = DateTime.Now.Year - reader.NgaySinh.Year;
                if (reader.NgaySinh.Date > DateTime.Now.AddYears(-age)) age--;

                if (age < minAge || age > maxAge)
                {
                    ModelState.AddModelError("NgaySinh", $"Độ tuổi hiện tại là {age}. Độc giả phải từ {minAge} đến {maxAge} tuổi.");
                }
            }

            if (ModelState.IsValid)
            {
                // Cập nhật lại ngày cấp và hạn thẻ bằng biến động một lần nữa cho chắc chắn
                reader.NgayCap = DateTime.Now;
                reader.NgayHetHan = DateTime.Now.AddMonths(cardMonths);

                _context.Add(reader);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Cấp thẻ thành công cho độc giả: {reader.HoTen}";
                return RedirectToAction(nameof(Index));
            }

            return View(reader);
        }
    }
}