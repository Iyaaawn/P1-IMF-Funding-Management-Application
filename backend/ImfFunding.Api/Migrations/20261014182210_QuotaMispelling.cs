using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImfFunding.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuotaMispelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QutoaAmountSdrMillions",
                table: "Countries",
                newName: "QuotaAmountSdrMillions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuotaAmountSdrMillions",
                table: "Countries",
                newName: "QutoaAmountSdrMillions");
        }
    }
}
