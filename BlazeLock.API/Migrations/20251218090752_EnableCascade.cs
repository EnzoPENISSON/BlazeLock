using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historique_entree_Entree_id_entree",
                table: "Historique_entree");

            migrationBuilder.AddForeignKey(
                name: "FK_Historique_entree_Entree_id_entree",
                table: "Historique_entree",
                column: "id_entree",
                principalTable: "Entree",
                principalColumn: "id_entree",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historique_entree_Entree_id_entree",
                table: "Historique_entree");

            migrationBuilder.AddForeignKey(
                name: "FK_Historique_entree_Entree_id_entree",
                table: "Historique_entree",
                column: "id_entree",
                principalTable: "Entree",
                principalColumn: "id_entree");
        }
    }
}
