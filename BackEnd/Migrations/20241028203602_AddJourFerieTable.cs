using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TarikhMaghribi.Migrations
{
    /// <inheritdoc />
    public partial class AddJourFerieTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "571c7e03-bb7b-4ef8-b9fd-5977d338d765");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90ea8116-aebf-4574-805d-edd59badeacc");

            migrationBuilder.CreateTable(
                name: "JoursFeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateJour = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoursFeries", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1479af3f-64a9-41dd-bbfe-99445eaee181", null, "superutilisateur", "SUPERUTILISATEUR" },
                    { "3e00c18d-18db-48cd-93c3-cf08acd1b1c7", null, "usernormal", "USERNORMAL" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoursFeries");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1479af3f-64a9-41dd-bbfe-99445eaee181");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3e00c18d-18db-48cd-93c3-cf08acd1b1c7");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "571c7e03-bb7b-4ef8-b9fd-5977d338d765", null, "usernormal", "USERNORMAL" },
                    { "90ea8116-aebf-4574-805d-edd59badeacc", null, "superutilisateur", "SUPERUTILISATEUR" }
                });
        }
    }
}
