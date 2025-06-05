using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDeals.Shared.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "Store",
                table: "Prices");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceNew",
                table: "Prices",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShopName",
                table: "Prices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Prices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceNew",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "ShopName",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Prices");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Prices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Store",
                table: "Prices",
                type: "TEXT",
                nullable: true);
        }
    }
}
