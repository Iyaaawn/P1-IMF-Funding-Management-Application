using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImfFunding.Api.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FundingRequests_RequestedById",
                table: "FundingRequests",
                column: "RequestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_FundingRequests_Users_RequestedById",
                table: "FundingRequests",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundingRequests_Users_RequestedById",
                table: "FundingRequests");

            migrationBuilder.DropIndex(
                name: "IX_FundingRequests_RequestedById",
                table: "FundingRequests");
        }
    }
}
