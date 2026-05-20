using CNPM_DeMo.Data;
using CNPM_DeMo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CNPM_DeMo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibManageDbContext _context;

        public HomeController(ILogger<HomeController> logger, LibManageDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Now;

            // 1. TÍNH TOÁN 4 CH? S? T?NG QUAN
            ViewBag.TotalBooks = await _context.Books.CountAsync();

            ViewBag.ActiveReaders = await _context.ReaderCards
                .CountAsync(r => r.NgayHetHan >= today);

            ViewBag.BorrowedBooks = await _context.BorrowDetails
                .CountAsync(d => d.DaTra == false);

            ViewBag.OverdueBooks = await _context.BorrowDetails
                .Include(d => d.BorrowTicket)
                .CountAsync(d => d.DaTra == false && d.BorrowTicket.NgayPhaiTra < today);

            // 2. D? LI?U BI?U ?? (7 NGÀY QUA)
            var sevenDaysAgo = today.AddDays(-6).Date;
            var recentTickets = await _context.BorrowTickets
                .Where(t => t.NgayMuon >= sevenDaysAgo)
                .ToListAsync();

            var chartLabels = new List<string>();
            var chartData = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                var date = sevenDaysAgo.AddDays(i);
                chartLabels.Add(date.ToString("dd/MM"));
                // ??m s? l??ng phi?u m??n trong t?ng ngày
                chartData.Add(recentTickets.Count(t => t.NgayMuon.Date == date));
            }

            ViewBag.ChartLabels = chartLabels;
            ViewBag.ChartData = chartData;

            // 3. L?Y HO?T ??NG G?N ?ÂY (5 l??t m?i nh?t)
            var recentActivities = await _context.BorrowDetails
                .Include(d => d.BorrowTicket).ThenInclude(t => t.ReaderCard)
                .Include(d => d.Book)
                .OrderByDescending(d => d.DaTra ? d.NgayTraThucTe : d.BorrowTicket.NgayMuon)
                .Take(5)
                .ToListAsync();

            return View(recentActivities);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}