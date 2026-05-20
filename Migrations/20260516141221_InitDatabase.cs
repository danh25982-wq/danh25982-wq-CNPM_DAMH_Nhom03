using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CNPM_DeMo.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhauMaHoa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSach = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenSach = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TacGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TheLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NhaXuatBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NamXuatBan = table.Column<int>(type: "int", nullable: false),
                    GiaTri = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReaderCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayCap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReaderCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoTuoiToiThieu = table.Column<int>(type: "int", nullable: false),
                    DoTuoiToiDa = table.Column<int>(type: "int", nullable: false),
                    ThoiHanTheThang = table.Column<int>(type: "int", nullable: false),
                    ThoiHanMuonNgay = table.Column<int>(type: "int", nullable: false),
                    SoSachMuonToiDa = table.Column<int>(type: "int", nullable: false),
                    DonGiaPhatNgay = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BorrowTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReaderCardId = table.Column<int>(type: "int", nullable: false),
                    NgayMuon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayPhaiTra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowTickets_ReaderCards_ReaderCardId",
                        column: x => x.ReaderCardId,
                        principalTable: "ReaderCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BorrowDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BorrowTicketId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    NgayTraThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TienPhatQuaHan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienPhatHuHai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TinhTrangKhiTra = table.Column<int>(type: "int", nullable: true),
                    DaTra = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowDetails_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BorrowDetails_BorrowTickets_BorrowTicketId",
                        column: x => x.BorrowTicketId,
                        principalTable: "BorrowTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "HoTen", "IsActive", "MatKhauMaHoa", "TenDangNhap", "VaiTro" },
                values: new object[] { 1, "Quản trị viên", true, "Admin@123", "admin", 0 });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "DoTuoiToiDa", "DoTuoiToiThieu", "DonGiaPhatNgay", "SoSachMuonToiDa", "ThoiHanMuonNgay", "ThoiHanTheThang" },
                values: new object[] { 1, 60, 18, 1000m, 5, 14, 6 });

            migrationBuilder.CreateIndex(
                name: "IX_Books_MaSach",
                table: "Books",
                column: "MaSach",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowDetails_BookId",
                table: "BorrowDetails",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowDetails_BorrowTicketId",
                table: "BorrowDetails",
                column: "BorrowTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowTickets_MaPhieu",
                table: "BorrowTickets",
                column: "MaPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowTickets_ReaderCardId",
                table: "BorrowTickets",
                column: "ReaderCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ReaderCards_MaThe",
                table: "ReaderCards",
                column: "MaThe",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "BorrowDetails");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "BorrowTickets");

            migrationBuilder.DropTable(
                name: "ReaderCards");
        }
    }
}
