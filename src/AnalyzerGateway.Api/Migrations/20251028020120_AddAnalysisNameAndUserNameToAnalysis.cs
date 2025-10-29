using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnalyzerGateway.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisNameAndUserNameToAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnalysisName",
                table: "Analysis",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Analysis",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalysisName",
                table: "Analysis");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Analysis");
        }
    }
}
