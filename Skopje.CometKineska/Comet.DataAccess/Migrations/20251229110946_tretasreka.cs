using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Comet.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class tretasreka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyerUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LibertyUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibertyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibertyUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LibertyUserId = table.Column<int>(type: "int", nullable: true),
                    BuyerUserId = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorTopSide = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ColorBottomSide = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZincCoating = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Thickness = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Defects = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuctionStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MinimumBidPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_BuyerUsers_BuyerUserId",
                        column: x => x.BuyerUserId,
                        principalTable: "BuyerUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_LibertyUsers_LibertyUserId",
                        column: x => x.LibertyUserId,
                        principalTable: "LibertyUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BuyerUserId = table.Column<int>(type: "int", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BidTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsWinningBid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bids_BuyerUsers_BuyerUserId",
                        column: x => x.BuyerUserId,
                        principalTable: "BuyerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bids_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AuctionStartTime", "BuyerUserId", "ColorBottomSide", "ColorTopSide", "CreatedAt", "Defects", "Grade", "GrossWeight", "IsPublished", "LibertyUserId", "MinimumBidPrice", "NetWeight", "Price", "ProductCategory", "ProductCode", "ProductType", "PublishedAt", "Thickness", "Width", "ZincCoating" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "RAL 9002 LTP", new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1778), "unpainted spots", "DX51D +Z", 1465m, false, null, 0m, 1465m, null, "FourthChoice", "2501998/11", "PPG_Coil", null, 0.75m, 1500, "Z 140" },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "LZS 7030", "RAL 9006", new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1790), "unpainted spots", "S250GD +Z", 0.890m, false, null, 0m, 0.880m, null, "FourthChoice", "2503658/41", "PPG_Coil", null, 0.50m, 1250, "Z 140" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Report" },
                    { 3, "Customer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Email", "ModifiedDate", "Password", "RoleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1846), "admin@liberty.com", null, "AQAAAAIAAYagAAAAEFPMtFeiolPvXsWN1g9fjrBV1hwQI65nOjnL/Z7QZtMbJ1NUjOWKrpg6JBC+oY5Kkg==", 1 },
                    { 2, new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1852), "reports@liberty.com", null, "AQAAAAIAAYagAAAAEI30L+x9SLK376ddrhdlCZSzVoF2i/7/jOmCcDIiLRoh+yVKQsrGaG0VlmJvgJR5+Q==", 2 },
                    { 3, new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1932), "customer1@buyer.com", null, "AQAAAAIAAYagAAAAEN5FiNedAWty+f2XEZ/urSVat2TzTklLWJUxEp5YBxMHSkizHvqk5hQ1PxESb/SIJw==", 3 },
                    { 4, new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1939), "customer2@buyer.com", null, "AQAAAAIAAYagAAAAEN5FiNedAWty+f2XEZ/urSVat2TzTklLWJUxEp5YBxMHSkizHvqk5hQ1PxESb/SIJw==", 3 }
                });

            migrationBuilder.InsertData(
                table: "BuyerUsers",
                columns: new[] { "Id", "CompanyName" },
                values: new object[,]
                {
                    { 3, "SteelWorks Inc" },
                    { 4, "MetalPro Industries" }
                });

            migrationBuilder.InsertData(
                table: "LibertyUsers",
                columns: new[] { "Id", "FullName" },
                values: new object[,]
                {
                    { 1, "Liberty Admin" },
                    { 2, "Report User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bids_BuyerUserId",
                table: "Bids",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_ProductId",
                table: "Bids",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerUsers_CompanyName",
                table: "BuyerUsers",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_LibertyUsers_FullName",
                table: "LibertyUsers",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BuyerUserId",
                table: "Products",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LibertyUserId",
                table: "Products",
                column: "LibertyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCode",
                table: "Products",
                column: "ProductCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "BuyerUsers");

            migrationBuilder.DropTable(
                name: "LibertyUsers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
