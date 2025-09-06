using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MandagsSpil.Api.Migrations.Persistence
{
    /// <inheritdoc />
    public partial class AddUserCod2UsernameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Cod2Username",
                table: "Users",
                column: "Cod2Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Cod2Username",
                table: "Users");
        }
    }
}
