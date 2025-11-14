using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnalyzerGateway.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefactoringSuggestionToModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefactoringSuggestion",
                table: "Modification",
                type: "TEXT",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefactoringSuggestion",
                table: "Modification");
        }
    }
}
