using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comet.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuctionEndDate",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AuctionEndDate", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 2, 13, 14, 18, 5, 234, DateTimeKind.Utc).AddTicks(7164) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AuctionEndDate", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 2, 13, 14, 18, 5, 234, DateTimeKind.Utc).AddTicks(7176) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2026, 2, 13, 14, 18, 5, 234, DateTimeKind.Utc).AddTicks(7217), "AQAAAAIAAYagAAAAEHVaBd7TgkI3iJxPzwMYQBOYpwQOKtefaVftOtxxsILnibw+EUncOR6Rsm+gPqiuZQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2026, 2, 13, 14, 18, 5, 234, DateTimeKind.Utc).AddTicks(7222), "AQAAAAIAAYagAAAAEB5MDVk7Q3BKkOhshxtqzZ0sR+5o47f1Rt5ciIgzzoah5oGYbtseAMp9hH+qJ6V6RA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2026, 2, 13, 14, 18, 5, 234, DateTimeKind.Utc).AddTicks(7246), "AQAAAAIAAYagAAAAEH+z762xHy+4qdyi0FGB92UfHg+olFccAJIPpHqs3mUaNSp1U28g95giaFAFSuzaNA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2026, 2, 13, 14, 18, 5, 234, DateTimeKind.Utc).AddTicks(7247), "AQAAAAIAAYagAAAAEH+z762xHy+4qdyi0FGB92UfHg+olFccAJIPpHqs3mUaNSp1U28g95giaFAFSuzaNA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionEndDate",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1778));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1790));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1846), "AQAAAAIAAYagAAAAEFPMtFeiolPvXsWN1g9fjrBV1hwQI65nOjnL/Z7QZtMbJ1NUjOWKrpg6JBC+oY5Kkg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1852), "AQAAAAIAAYagAAAAEI30L+x9SLK376ddrhdlCZSzVoF2i/7/jOmCcDIiLRoh+yVKQsrGaG0VlmJvgJR5+Q==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1932), "AQAAAAIAAYagAAAAEN5FiNedAWty+f2XEZ/urSVat2TzTklLWJUxEp5YBxMHSkizHvqk5hQ1PxESb/SIJw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "Password" },
                values: new object[] { new DateTime(2025, 12, 29, 11, 9, 45, 440, DateTimeKind.Utc).AddTicks(1939), "AQAAAAIAAYagAAAAEN5FiNedAWty+f2XEZ/urSVat2TzTklLWJUxEp5YBxMHSkizHvqk5hQ1PxESb/SIJw==" });
        }
    }
}
