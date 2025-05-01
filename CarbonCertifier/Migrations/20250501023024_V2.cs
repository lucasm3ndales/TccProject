using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbonCertifier.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedAt",
                table: "CarbonCredits",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedAt",
                table: "CarbonCredits",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CarbonCredits");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CarbonCredits");
        }
    }
}
