using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopMgmt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShopAndMaterialUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Shops");

            migrationBuilder.RenameColumn(
                name: "FlightNumber",
                table: "MaterialUsages",
                newName: "TailNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TailNumber",
                table: "MaterialUsages",
                newName: "FlightNumber");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
