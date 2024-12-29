using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarbonCertifier.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarbonProjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectCode = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Developer = table.Column<string>(type: "text", nullable: false),
                    Methodology = table.Column<string>(type: "text", nullable: false),
                    EstimateEmissionReductions = table.Column<double>(type: "double precision", nullable: true),
                    EmissionReductions = table.Column<double>(type: "double precision", nullable: true),
                    CertificationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CertificationExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CertificationKey = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarbonProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarbonCredits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreditCode = table.Column<Guid>(type: "uuid", nullable: false),
                    VintageYear = table.Column<int>(type: "integer", nullable: false),
                    TonCO2Quantity = table.Column<double>(type: "double precision", nullable: false),
                    IsRetired = table.Column<bool>(type: "boolean", nullable: false),
                    RetireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PricePerTon = table.Column<double>(type: "double precision", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    CarbonProjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarbonCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarbonCredits_CarbonProjects_CarbonProjectId",
                        column: x => x.CarbonProjectId,
                        principalTable: "CarbonProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarbonCredits_CarbonProjectId",
                table: "CarbonCredits",
                column: "CarbonProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CarbonCredits_CreditCode",
                table: "CarbonCredits",
                column: "CreditCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarbonProjects_ProjectCode",
                table: "CarbonProjects",
                column: "ProjectCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarbonCredits");

            migrationBuilder.DropTable(
                name: "CarbonProjects");
        }
    }
}
