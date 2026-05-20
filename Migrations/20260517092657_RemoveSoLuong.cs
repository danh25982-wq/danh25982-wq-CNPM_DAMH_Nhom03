using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CNPM_DeMo.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoLuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuong",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLuong",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
