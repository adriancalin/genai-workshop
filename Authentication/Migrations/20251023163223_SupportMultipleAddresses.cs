using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Migrations
{
    /// <inheritdoc />
    public partial class SupportMultipleAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_CompanyId",
                schema: "business",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CompanyId",
                schema: "business",
                table: "Addresses",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_CompanyId",
                schema: "business",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CompanyId",
                schema: "business",
                table: "Addresses",
                column: "CompanyId",
                unique: true,
                filter: "[CompanyId] IS NOT NULL");
        }
    }
}
