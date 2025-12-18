using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class emailUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Utilisateur",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "Utilisateur");
        }
    }
}
