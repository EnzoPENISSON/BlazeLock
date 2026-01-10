using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class KeepLogsOnDeleteCoffre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Log_Coffre_id_coffre",
                table: "Log");

            migrationBuilder.AlterColumn<Guid>(
                name: "id_coffre",
                table: "Log",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Coffre_id_coffre",
                table: "Log",
                column: "id_coffre",
                principalTable: "Coffre",
                principalColumn: "id_coffre",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
