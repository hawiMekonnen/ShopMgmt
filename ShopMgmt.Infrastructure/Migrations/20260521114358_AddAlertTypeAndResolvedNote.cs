using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopMgmt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertTypeAndResolvedNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Alerts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResolvedNote",
                table: "Alerts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "ResolvedNote",
                table: "Alerts");
        }
    }
}
