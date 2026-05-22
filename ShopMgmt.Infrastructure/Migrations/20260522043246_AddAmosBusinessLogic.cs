using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopMgmt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAmosBusinessLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StockBatches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "StockBatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CollectedByUserId",
                table: "MaterialUsages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IssuedByUserId",
                table: "MaterialUsages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "MaterialUsages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AircraftTypes",
                table: "Materials",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultShopId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Materials",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinStock",
                table: "Materials",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "Materials",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReorderNote",
                table: "Materials",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReorderPlaced",
                table: "Materials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Alerts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Alerts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaterialRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AircraftOrWorkOrder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadyAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_MaterialRequests_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialRequests_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialReturns",
                columns: table => new
                {
                    ReturnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    UsageId = table.Column<int>(type: "int", nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    ReturnedByUserId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReturns", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_MaterialReturns_MaterialUsages_UsageId",
                        column: x => x.UsageId,
                        principalTable: "MaterialUsages",
                        principalColumn: "UsageId");
                    table.ForeignKey(
                        name: "FK_MaterialReturns_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialReturns_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialReturns_StockBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "StockBatches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaterialReturns_Users_ReturnedByUserId",
                        column: x => x.ReturnedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockBatches_ShopId",
                table: "StockBatches",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBatches_Status",
                table: "StockBatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialUsages_RequestId",
                table: "MaterialUsages",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_DefaultShopId",
                table: "Materials",
                column: "DefaultShopId");

            migrationBuilder.Sql(
                "UPDATE Materials SET PartNumber = CONCAT('LEGACY-', MaterialId) WHERE PartNumber = '' OR PartNumber IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_PartNumber",
                table: "Materials",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_RequestId",
                table: "Alerts",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_MaterialId",
                table: "MaterialRequests",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_RequestedByUserId",
                table: "MaterialRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_ShopId",
                table: "MaterialRequests",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReturns_BatchId",
                table: "MaterialReturns",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReturns_MaterialId",
                table: "MaterialReturns",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReturns_ReturnedByUserId",
                table: "MaterialReturns",
                column: "ReturnedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReturns_ShopId",
                table: "MaterialReturns",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReturns_UsageId",
                table: "MaterialReturns",
                column: "UsageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_MaterialRequests_RequestId",
                table: "Alerts",
                column: "RequestId",
                principalTable: "MaterialRequests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Shops_DefaultShopId",
                table: "Materials",
                column: "DefaultShopId",
                principalTable: "Shops",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialUsages_MaterialRequests_RequestId",
                table: "MaterialUsages",
                column: "RequestId",
                principalTable: "MaterialRequests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockBatches_Shops_ShopId",
                table: "StockBatches",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_MaterialRequests_RequestId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Shops_DefaultShopId",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialUsages_MaterialRequests_RequestId",
                table: "MaterialUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_StockBatches_Shops_ShopId",
                table: "StockBatches");

            migrationBuilder.DropTable(
                name: "MaterialRequests");

            migrationBuilder.DropTable(
                name: "MaterialReturns");

            migrationBuilder.DropIndex(
                name: "IX_StockBatches_ShopId",
                table: "StockBatches");

            migrationBuilder.DropIndex(
                name: "IX_StockBatches_Status",
                table: "StockBatches");

            migrationBuilder.DropIndex(
                name: "IX_MaterialUsages_RequestId",
                table: "MaterialUsages");

            migrationBuilder.DropIndex(
                name: "IX_Materials_DefaultShopId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_PartNumber",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_RequestId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "StockBatches");

            migrationBuilder.DropColumn(
                name: "CollectedByUserId",
                table: "MaterialUsages");

            migrationBuilder.DropColumn(
                name: "IssuedByUserId",
                table: "MaterialUsages");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "MaterialUsages");

            migrationBuilder.DropColumn(
                name: "AircraftTypes",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "DefaultShopId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "MinStock",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ReorderNote",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ReorderPlaced",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Alerts");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "StockBatches",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Alerts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
