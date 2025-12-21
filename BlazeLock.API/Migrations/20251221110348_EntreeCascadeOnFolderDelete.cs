using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class EntreeCascadeOnFolderDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dossier_Coffre_id_coffre",
                table: "Dossier");

            migrationBuilder.DropForeignKey(
                name: "FK_Entree_Dossier_id_dossier",
                table: "Entree");

            migrationBuilder.AddForeignKey(
                name: "FK_Dossier_Coffre_id_coffre",
                table: "Dossier",
                column: "id_coffre",
                principalTable: "Coffre",
                principalColumn: "id_coffre",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Entree_Dossier_id_dossier",
                table: "Entree",
                column: "id_dossier",
                principalTable: "Dossier",
                principalColumn: "Id_dossier",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dossier_Coffre_id_coffre",
                table: "Dossier");

            migrationBuilder.DropForeignKey(
                name: "FK_Entree_Dossier_id_dossier",
                table: "Entree");

            migrationBuilder.AddForeignKey(
                name: "FK_Dossier_Coffre_id_coffre",
                table: "Dossier",
                column: "id_coffre",
                principalTable: "Coffre",
                principalColumn: "id_coffre");

            migrationBuilder.AddForeignKey(
                name: "FK_Entree_Dossier_id_dossier",
                table: "Entree",
                column: "id_dossier",
                principalTable: "Dossier",
                principalColumn: "Id_dossier");
        }
    }
}
