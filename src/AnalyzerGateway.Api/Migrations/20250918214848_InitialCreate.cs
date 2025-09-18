using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnalyzerGateway.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analysis",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Tolerance = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Language = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    WhatHeSee = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analysis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalysisId = table.Column<string>(type: "TEXT", nullable: false),
                    Devolution = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modification_Analysis_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analysis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analysis_CreatedAtUtc",
                table: "Analysis",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Analysis_Url",
                table: "Analysis",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_Modification_AnalysisId",
                table: "Modification",
                column: "AnalysisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modification");

            migrationBuilder.DropTable(
                name: "Analysis");
        }
    }
}
