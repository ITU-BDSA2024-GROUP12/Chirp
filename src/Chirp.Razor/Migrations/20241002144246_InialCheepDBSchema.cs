using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class InialCheepDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => new { x.Name, x.Email });
                });

            migrationBuilder.CreateTable(
                name: "Cheeps",
                columns: table => new
                {
                    CheepId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    text = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorEmail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cheeps", x => x.CheepId);
                    table.ForeignKey(
                        name: "FK_Cheeps_Authors_AuthorName_AuthorEmail",
                        columns: x => new { x.AuthorName, x.AuthorEmail },
                        principalTable: "Authors",
                        principalColumns: new[] { "Name", "Email" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cheeps_AuthorName_AuthorEmail",
                table: "Cheeps",
                columns: new[] { "AuthorName", "AuthorEmail" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cheeps");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
