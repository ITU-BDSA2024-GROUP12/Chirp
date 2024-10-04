using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "text",
                table: "Cheeps",
                newName: "Text");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Cheeps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Cheeps");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Cheeps",
                newName: "text");
        }
    }
}
