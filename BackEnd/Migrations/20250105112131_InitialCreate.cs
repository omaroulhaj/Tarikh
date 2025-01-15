using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TarikhMaghribi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "582dc891-666a-4129-abab-cfdacdb03108");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7185f12-395f-44ce-a743-6383d6cb35c8");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "545f5d09-817e-4e1d-822d-38eac853ac01", null, "superutilisateur", "SUPERUTILISATEUR" },
                    { "acf79400-32dc-4005-8276-7cf5651c9c34", null, "usernormal", "USERNORMAL" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "545f5d09-817e-4e1d-822d-38eac853ac01");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "acf79400-32dc-4005-8276-7cf5651c9c34");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "582dc891-666a-4129-abab-cfdacdb03108", null, "usernormal", "USERNORMAL" },
                    { "a7185f12-395f-44ce-a743-6383d6cb35c8", null, "superutilisateur", "SUPERUTILISATEUR" }
                });
        }
    }
}
