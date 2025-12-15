using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class libstring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "libelle_tag",
                table: "Historique_entree");

            migrationBuilder.DropColumn(
                name: "libelle_vi",
                table: "Historique_entree");

            migrationBuilder.AlterColumn<string>(
                name: "libelle",
                table: "Historique_entree",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "libelle",
                table: "Coffre",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "libelle",
                table: "Historique_entree",
                type: "varbinary(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<byte[]>(
                name: "libelle_tag",
                table: "Historique_entree",
                type: "varbinary(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "libelle_vi",
                table: "Historique_entree",
                type: "varbinary(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<byte[]>(
                name: "libelle",
                table: "Coffre",
                type: "varbinary(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
