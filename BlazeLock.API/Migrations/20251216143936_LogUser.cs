using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class LogUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdUtilisateur",
                table: "Log",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UtilisateurIdUtilisateur",
                table: "Log",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Log_UtilisateurIdUtilisateur",
                table: "Log",
                column: "UtilisateurIdUtilisateur");

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Utilisateur_UtilisateurIdUtilisateur",
                table: "Log",
                column: "UtilisateurIdUtilisateur",
                principalTable: "Utilisateur",
                principalColumn: "id_utilisateur",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Log_Utilisateur_UtilisateurIdUtilisateur",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_UtilisateurIdUtilisateur",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "IdUtilisateur",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "UtilisateurIdUtilisateur",
                table: "Log");
        }
    }
}
