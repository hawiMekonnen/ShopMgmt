using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopMgmt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialTechnicianVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HiddenFromTechnicians",
                table: "Materials",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HiddenFromTechnicians",
                table: "Materials");
        }
    }
}
