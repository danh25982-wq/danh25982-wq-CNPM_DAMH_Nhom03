using CNPM_DeMo.Data;
using CNPM_DeMo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CNPM_DeMo.Controllers
{
    [Authorize]
    public class BorrowController : Controller
    {
        private readonly LibManageDbContext _context;

        public BorrowController(LibManageDbContext context)
        {
            _context = context;
        }

        // 1. HIỂN THỊ DANH SÁCH CÁC PHIẾU ĐÃ MƯỢN
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.BorrowTickets
                .Include(t => t.ReaderCard)
                .OrderByDescending(t => t.NgayMuon)
                .ToListAsync();
            return View(tickets);
        }

        // 2. GIAO DIỆN LẬP PHIẾU MƯỢN MỚI
        public IActionResult Create()
        {
            return View();
        }

        // ==========================================
        // CÁC HÀM API HỖ TRỢ AJAX 
        // ==========================================

        // 3. API TÌM KIẾM ĐỘC GIẢ 
        [HttpGet]
        public async Task<IActionResult> SearchReaders(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return BadRequest();

            var readers = await _context.ReaderCards
                .Where(r => r.MaThe.Contains(keyword) || r.HoTen.Contains(keyword))
                .Select(r => new {
                    r.Id,
                    r.MaThe,
                    r.HoTen,
                    IsExpired = r.NgayHetHan < DateTime.Now
                })
                .ToListAsync();

            return Json(readers);
        }

        // 4. API XÁC NHẬN ĐỘC GIẢ ĐƯỢC CHỌN (Đã tích hợp SystemSetting)
        [HttpGet]
        public async Task<IActionResult> SelectReader(int id)
        {
            var reader = await _context.ReaderCards.FindAsync(id);
            if (reader == null) return Json(new { success = false, message = "Không tìm thấy độc giả!" });
            if (reader.NgayHetHan < DateTime.Now) return Json(new { success = false, message = "Thẻ độc giả này đã hết hạn sử dụng!" });

            // Gọi cấu hình hệ thống
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            int maxBooks = setting?.SoSachMuonToiDa ?? 5;

            var borrowedCount = await _context.BorrowDetails
                .Include(d => d.BorrowTicket)
                .Where(d => d.BorrowTicket.ReaderCardId == reader.Id && !d.DaTra)
                .CountAsync();

            if (borrowedCount >= maxBooks)
                return Json(new { success = false, message = $"Độc giả này đã mượn tối đa {maxBooks} quyển, không thể mượn thêm!" });

            return Json(new { success = true, id = reader.Id, hoTen = reader.HoTen, dangMuon = borrowedCount });
        }

        // 5. API TÌM KIẾM SÁCH 
        [HttpGet]
        public async Task<IActionResult> SearchBooks(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return BadRequest();

            var books = await _context.Books
                .Where(b => b.MaSach.Contains(keyword) || b.TenSach.Contains(keyword))
                .Select(b => new {
                    b.Id,
                    b.MaSach,
                    b.TenSach,
                    b.TrangThai
                })
                .ToListAsync();

            return Json(books);
        }

        // 6. API XÁC NHẬN SÁCH ĐƯỢC CHỌN 
        [HttpGet]
        public async Task<IActionResult> SelectBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return Json(new { success = false, message = "Không tìm thấy sách!" });

            if (book.TrangThai != BookStatus.SanSang)
                return Json(new { success = false, message = $"Sách '{book.TenSach}' hiện không sẵn sàng để mượn!" });

            return Json(new { success = true, id = book.Id, maSach = book.MaSach, tenSach = book.TenSach });
        }

        public IActionResult Return()
        {
            return View();
        }

        // 8.5. API TÌM KIẾM ĐA NĂNG
        [HttpGet]
        public async Task<IActionResult> SearchActiveTickets(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return BadRequest();

            var tickets = await _context.BorrowTickets
                .Include(t => t.ReaderCard)
                .Include(t => t.BorrowDetails).ThenInclude(d => d.Book)
                .Where(t => t.BorrowDetails.Any(d => !d.DaTra) &&
                            (t.MaPhieu.Contains(keyword) ||
                             t.ReaderCard.MaThe.Contains(keyword) ||
                             t.ReaderCard.HoTen.Contains(keyword) ||
                             t.BorrowDetails.Any(d => !d.DaTra && (d.Book.MaSach.Contains(keyword) || d.Book.TenSach.Contains(keyword)))))
                .Select(t => new {
                    maPhieu = t.MaPhieu,
                    ngayMuon = t.NgayMuon.ToString("dd/MM/yyyy"),
                    hoTen = t.ReaderCard.HoTen,
                    soSachChuaTra = t.BorrowDetails.Count(d => !d.DaTra)
                })
                .ToListAsync();

            return Json(tickets);
        }

        // 9. API TÌM KIẾM CHI TIẾT PHIẾU MƯỢN ĐỂ TRẢ
        [HttpGet]
        public async Task<IActionResult> SearchTicketToReturn(string maPhieu)
        {
            if (string.IsNullOrEmpty(maPhieu)) return BadRequest();

            var ticket = await _context.BorrowTickets
                .Include(t => t.ReaderCard)
                .Include(t => t.BorrowDetails).ThenInclude(d => d.Book)
                .FirstOrDefaultAsync(t => t.MaPhieu == maPhieu);

            if (ticket == null)
                return Json(new { success = false, message = "Không tìm thấy mã phiếu mượn này!" });

            var unreturnedBooks = ticket.BorrowDetails.Where(d => !d.DaTra).Select(d => new {
                detailId = d.Id,
                maSach = d.Book.MaSach,
                tenSach = d.Book.TenSach,
                giaTri = d.Book.GiaTri,
                ngayPhaiTra = ticket.NgayPhaiTra.ToString("dd/MM/yyyy"),
                overdueDays = (DateTime.Now.Date - ticket.NgayPhaiTra.Date).Days > 0
                              ? (DateTime.Now.Date - ticket.NgayPhaiTra.Date).Days : 0
            }).ToList();

            if (!unreturnedBooks.Any())
                return Json(new { success = false, message = "Phiếu mượn này đã được hoàn tất trả toàn bộ sách!" });

            return Json(new
            {
                success = true,
                readerName = ticket.ReaderCard.HoTen,
                ticketDate = ticket.NgayMuon.ToString("dd/MM/yyyy"),
                details = unreturnedBooks
            });
        }

        // 10. API XỬ LÝ TRẢ TỪNG CUỐN SÁCH VÀ TÍNH PHẠT (Đã tích hợp SystemSetting)
        [HttpPost]
        public async Task<IActionResult> ProcessReturnBook(int detailId, int conditionStatus)
        {
            var detail = await _context.BorrowDetails
                .Include(d => d.Book)
                .Include(d => d.BorrowTicket)
                .FirstOrDefaultAsync(d => d.Id == detailId);

            if (detail == null || detail.DaTra)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ hoặc sách đã được trả!" });

            // Gọi cấu hình hệ thống
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            decimal finePerDay = setting?.DonGiaPhatNgay ?? 1000;

            detail.NgayTraThucTe = DateTime.Now;
            detail.DaTra = true;
            detail.TinhTrangKhiTra = (ReturnCondition)conditionStatus;

            // 1. Tính toán chi phí phạt trễ hạn dựa vào cấu hình động
            int overdueDays = (DateTime.Now.Date - detail.BorrowTicket.NgayPhaiTra.Date).Days;
            if (overdueDays > 0)
            {
                detail.TienPhatQuaHan = overdueDays * finePerDay;
            }

            // 2. Tính toán chi phí phạt hư hại vật lý
            decimal phanTramPhat = 0;
            string tinhTrangStr = "Bình thường";

            if (detail.TinhTrangKhiTra == ReturnCondition.HuHongNhe) { phanTramPhat = 0.3m; tinhTrangStr = "Hư hỏng nhẹ (Phạt 30%)"; }
            else if (detail.TinhTrangKhiTra == ReturnCondition.HuHongNang) { phanTramPhat = 0.5m; tinhTrangStr = "Hư hỏng nặng (Phạt 50%)"; }
            else if (detail.TinhTrangKhiTra == ReturnCondition.MatSach) { phanTramPhat = 1.0m; tinhTrangStr = "Mất sách (Phạt 100%)"; }

            detail.TienPhatHuHai = detail.Book.GiaTri * phanTramPhat;

            // CẬP NHẬT GHI CHÚ VÀO PHIẾU MƯỢN TỔNG
            var dongNhatKy = $"[Đã trả: {detail.Book.MaSach} - {tinhTrangStr} - Ngày: {DateTime.Now:dd/MM/yyyy}]";
            if (string.IsNullOrEmpty(detail.BorrowTicket.GhiChu))
            {
                detail.BorrowTicket.GhiChu = dongNhatKy;
            }
            else
            {
                detail.BorrowTicket.GhiChu += " | " + dongNhatKy;
            }

            // 3. Điều tiết lại số lượng hàng tồn kho vật lý
            if (detail.TinhTrangKhiTra == ReturnCondition.BinhThuong || detail.TinhTrangKhiTra == ReturnCondition.HuHongNhe)
            {
                detail.Book.TrangThai = BookStatus.SanSang;
            }
            else if (detail.TinhTrangKhiTra == ReturnCondition.HuHongNang)
            {
                detail.Book.TrangThai = BookStatus.HuHong;
            }
            else if (detail.TinhTrangKhiTra == ReturnCondition.MatSach)
            {
                detail.Book.TrangThai = BookStatus.NgungLuuHanh;
            }

            _context.BorrowDetails.Update(detail);
            _context.BorrowTickets.Update(detail.BorrowTicket);
            _context.Books.Update(detail.Book);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                tienPhatTre = detail.TienPhatQuaHan,
                tienPhatHu = detail.TienPhatHuHai,
                tongPhat = detail.TienPhatQuaHan + detail.TienPhatHuHai
            });
        }

        // 7. XỬ LÝ LƯU PHIẾU MƯỢN (Đã tích hợp SystemSetting)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ReaderCardId, List<int> SelectedBookIds)
        {
            if (ReaderCardId <= 0 || SelectedBookIds == null || !SelectedBookIds.Any())
            {
                TempData["ErrorMessage"] = "Lỗi: Vui lòng xác nhận độc giả và chọn ít nhất 1 cuốn sách!";
                return RedirectToAction(nameof(Create));
            }

            // Gọi cấu hình hệ thống để lấy số ngày mượn
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            int borrowDays = setting?.ThoiHanMuonNgay ?? 14;

            var ticket = new BorrowTicket
            {
                MaPhieu = "PM" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                ReaderCardId = ReaderCardId,
                NgayMuon = DateTime.Now,
                NgayPhaiTra = DateTime.Now.AddDays(borrowDays), // Tính ngày trả động
                GhiChu = ""
            };

            _context.BorrowTickets.Add(ticket);
            await _context.SaveChangesAsync();

            foreach (var bookId in SelectedBookIds)
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null || book.TrangThai != BookStatus.SanSang) continue;

                var detail = new BorrowDetail
                {
                    BorrowTicketId = ticket.Id,
                    BookId = bookId,
                    DaTra = false
                };
                _context.BorrowDetails.Add(detail);

                book.TrangThai = BookStatus.DangMuon;
                _context.Books.Update(book);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Lập phiếu mượn thành công! Mã phiếu: {ticket.MaPhieu}";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // KHỐI XEM CHI TIẾT & GIA HẠN
        // ==========================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.BorrowTickets
                .Include(t => t.ReaderCard)
                .Include(t => t.BorrowDetails)
                    .ThenInclude(d => d.Book)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> RenewTicket(int id)
        {
            var ticket = await _context.BorrowTickets.FindAsync(id);
            if (ticket == null)
                return Json(new { success = false, message = "Không tìm thấy phiếu mượn!" });

            // Mặc định cho phép gia hạn thêm 7 ngày
            ticket.NgayPhaiTra = ticket.NgayPhaiTra.AddDays(7);

            var logText = $"[Gia hạn +7 ngày lúc {DateTime.Now:dd/MM/yyyy HH:mm}]";
            ticket.GhiChu = string.IsNullOrEmpty(ticket.GhiChu) ? logText : ticket.GhiChu + " | " + logText;

            _context.BorrowTickets.Update(ticket);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                newDate = ticket.NgayPhaiTra.ToString("dd/MM/yyyy"),
                message = "Gia hạn thành công thêm 7 ngày!"
            });
        }
        [HttpGet]
        public async Task<IActionResult> PrintTicket(int id)
        {
            var ticket = await _context.BorrowTickets
                .Include(t => t.ReaderCard)
                .Include(t => t.BorrowDetails)
                    .ThenInclude(d => d.Book)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }
    }
}