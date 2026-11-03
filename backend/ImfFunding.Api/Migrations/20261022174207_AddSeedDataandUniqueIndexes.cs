using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ImfFunding.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataandUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IsoCode",
                table: "Countries",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "IsoCode", "Name", "QuotaAmountSdrMillions" },
                values: new object[,]
                {
                    { 1, "GHA", "Ghana", 738.0m },
                    { 2, "KEN", "Kenya", 542.8m },
                    { 3, "ARG", "Argentina", 3187.3m },
                    { 4, "EGY", "Egypt", 2037.1m },
                    { 5, "PAK", "Pakistan", 2031.0m },
                    { 6, "LKA", "Sri Lanka", 578.8m }
                });

            migrationBuilder.InsertData(
                table: "LendingFacilities",
                columns: new[] { "Id", "CodeName", "Description", "FullName", "MaxTermMonths" },
                values: new object[,]
                {
                    { 1, "SBA", "Short-term support for countries with balance-of-payments problems.", "Stand-By Arrangement", 36 },
                    { 2, "EFF", "Longer-term support for countries with deep structural problems.", "Extended Fund Facility", 48 },
                    { 3, "ECF", "Concessional (low-interest) support for low-income countries.", "Extended Credit Facility", 60 },
                    { 4, "FCL", "Precautionary credit line for countries with very strong policies.", "Flexible Credit Line", 24 },
                    { 5, "RSF", "Long-term financing for climate and pandemic resilience.", "Resilience and Sustainability Facility", 36 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LendingFacilities_CodeName",
                table: "LendingFacilities",
                column: "CodeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_IsoCode",
                table: "Countries",
                column: "IsoCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LendingFacilities_CodeName",
                table: "LendingFacilities");

            migrationBuilder.DropIndex(
                name: "IX_Countries_IsoCode",
                table: "Countries");

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "LendingFacilities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LendingFacilities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LendingFacilities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LendingFacilities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LendingFacilities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<string>(
                name: "IsoCode",
                table: "Countries",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);
        }
    }
}
