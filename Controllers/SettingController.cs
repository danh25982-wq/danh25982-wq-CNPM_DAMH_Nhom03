using CNPM_DeMo.Data;
using CNPM_DeMo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CNPM_DeMo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly LibManageDbContext _context;

        public SettingController(LibManageDbContext context)
        {
            _context = context;
        }

        // 1. GIAO DIỆN HIỂN THỊ CẤU HÌNH
        public async Task<IActionResult> Index()
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();

            // Nếu database chưa có dòng nào, tự tạo 1 dòng mặc định đẩy lên View
            if (setting == null)
            {
                setting = new SystemSetting();
            }
            return View(setting);
        }

        // 2. LƯU THAY ĐỔI CẤU HÌNH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SystemSetting model)
        {
            if (ModelState.IsValid)
            {
                var setting = await _context.SystemSettings.FirstOrDefaultAsync();

                if (setting == null)
                {
                    // Nếu chưa có thì thêm mới
                    _context.SystemSettings.Add(model);
                }
                else
                {
                    // Nếu có rồi thì cập nhật đúng các trường của bạn
                    setting.DoTuoiToiThieu = model.DoTuoiToiThieu;
                    setting.DoTuoiToiDa = model.DoTuoiToiDa;
                    setting.ThoiHanTheThang = model.ThoiHanTheThang;
                    setting.ThoiHanMuonNgay = model.ThoiHanMuonNgay;
                    setting.SoSachMuonToiDa = model.SoSachMuonToiDa;
                    setting.DonGiaPhatNgay = model.DonGiaPhatNgay;

                    _context.Update(setting);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã cập nhật quy định thư viện thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}