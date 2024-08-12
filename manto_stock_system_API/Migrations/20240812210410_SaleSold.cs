using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace manto_stock_system_API.Migrations
{
    /// <inheritdoc />
    public partial class SaleSold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Sold",
                table: "Sales",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sold",
                table: "Sales");
        }
    }
}
