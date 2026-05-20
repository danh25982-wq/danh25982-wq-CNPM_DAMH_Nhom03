using Microsoft.EntityFrameworkCore;
using CNPM_DeMo.Models;

namespace CNPM_DeMo.Data
{
    public class LibManageDbContext : DbContext
    {
        public LibManageDbContext(DbContextOptions<LibManageDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng sẽ được tạo trong SQL Server
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ReaderCard> ReaderCards { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowTicket> BorrowTickets { get; set; }
        public DbSet<BorrowDetail> BorrowDetails { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình các cột Unique (Không được trùng lặp)
            modelBuilder.Entity<ReaderCard>().HasIndex(r => r.MaThe).IsUnique();
            modelBuilder.Entity<Book>().HasIndex(b => b.MaSach).IsUnique();
            modelBuilder.Entity<BorrowTicket>().HasIndex(t => t.MaPhieu).IsUnique();

            // 2. Cấu hình khóa ngoại và luật xóa (Cascade / Restrict)
            modelBuilder.Entity<BorrowDetail>()
                .HasOne(d => d.BorrowTicket)
                .WithMany(t => t.BorrowDetails)
                .HasForeignKey(d => d.BorrowTicketId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa phiếu mượn -> Xóa luôn chi tiết

            modelBuilder.Entity<BorrowDetail>()
                .HasOne(d => d.Book)
                .WithMany(b => b.BorrowDetails)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Restrict); // Sách đang có người mượn -> Không được xóa sách khỏi kho

            // 3. Đổ dữ liệu mẫu (Data Seeding) khi vừa tạo Database xong
            modelBuilder.Entity<SystemSetting>().HasData(
                new SystemSetting
                {
                    Id = 1,
                    DoTuoiToiThieu = 18,
                    DoTuoiToiDa = 60,
                    ThoiHanTheThang = 6,
                    ThoiHanMuonNgay = 14,
                    SoSachMuonToiDa = 5,
                    DonGiaPhatNgay = 1000
                }
            );

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    TenDangNhap = "admin",
                    MatKhauMaHoa = "Admin@123", // Mật khẩu đăng nhập mặc định
                    HoTen = "Quản trị viên",
                    VaiTro = SystemRole.Admin,
                    IsActive = true
                }
            );
        }
    }
}