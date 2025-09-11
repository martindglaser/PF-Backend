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
                name: "Analisis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Tolerancia = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Lenguage = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    WhatHeSee = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Devolucion = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analisis", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_CreatedAtUtc",
                table: "Analisis",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_Url",
                table: "Analisis",
                column: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analisis");
        }
    }
}
