using CNPM_DeMo.Data;
using CNPM_DeMo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CNPM_DeMo.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly LibManageDbContext _context;

        public BookController(LibManageDbContext context)
        {
            _context = context;
        }

        // 1. CHỨC NĂNG TRA CỨU & HIỂN THỊ (Hỗ trợ lọc theo Tên/Mã, Thể loại, Tác giả)
        public async Task<IActionResult> Index(string searchString, string category, string author)
        {
            // Lấy danh sách Thể loại và Tác giả duy nhất để nạp vào Dropdown list trên View
            ViewBag.Categories = await _context.Books.Select(b => b.TheLoai).Distinct().ToListAsync();
            ViewBag.Authors = await _context.Books.Select(b => b.TacGia).Distinct().ToListAsync();

            // Khởi tạo truy vấn LINQ
            var query = _context.Books.AsQueryable();

            // Áp dụng các bộ lọc đa chiều nếu có dữ liệu truyền vào
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.TenSach.Contains(searchString) || b.MaSach.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.TheLoai == category);
            }
            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.TacGia == author);
            }

            // Thực thi truy vấn và trả về View
            var books = await query.OrderByDescending(b => b.Id).ToListAsync();
            return View(books);
        }

        // 2. CHỨC NĂNG THÊM MỚI (GET: Hiển thị form)
        public IActionResult Create()
        {
            return View();
        }

        // 2. CHỨC NĂNG THÊM MỚI (POST: Xử lý lưu dữ liệu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            // BỎ QUA KIỂM TRA NHỮNG TRƯỜNG KHÔNG NHẬP TỪ GIAO DIỆN
            ModelState.Remove("BorrowDetails");
            ModelState.Remove("ViTri");
            ModelState.Remove("TrangThai"); // Bỏ qua Trạng thái để code tự gán

            // Kiểm tra trùng Mã sách
            if (await _context.Books.AnyAsync(b => b.MaSach == book.MaSach))
            {
                ModelState.AddModelError("MaSach", "Mã sách này đã tồn tại trong hệ thống!");
            }

            if (ModelState.IsValid)
            {
                book.TrangThai = BookStatus.SanSang;

                _context.Add(book);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Lưu thành công sách: {book.TenSach}";
                return RedirectToAction(nameof(Create));
            }
            else
            {
                // TUYỆT CHIÊU BẮT LỖI: Bắt hệ thống in ra chính xác tên ô bị lỗi
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Hệ thống chặn lưu vì: " + string.Join(" | ", errors);
            }

            return View(book);
        }

        // 3. CHỨC NĂNG SỬA THÔNG TIN (GET: Hiển thị form kèm dữ liệu cũ)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // 3. CHỨC NĂNG SỬA THÔNG TIN (POST: Cập nhật dữ liệu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();

            // BỎ QUA KIỂM TRA ĐỂ TRÁNH LỖI XÁC THỰC ẨN KHÓA NGOẠI
            ModelState.Remove("BorrowDetails");
            ModelState.Remove("ViTri");

            // Kiểm tra trùng mã sách với các cuốn khác
            if (await _context.Books.AnyAsync(b => b.MaSach == book.MaSach && b.Id != book.Id))
            {
                ModelState.AddModelError("MaSach", "Mã sách này đã bị trùng với một cuốn khác!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
        // 4. CHỨC NĂNG XEM CHI TIẾT
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin sách dựa vào Id
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
    }
}