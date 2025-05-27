using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbonCertifier.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "CarbonCredits",
                newName: "OwnerName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerName",
                table: "CarbonCredits",
                newName: "Owner");
        }
    }
}
