using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class addDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    id_utilisateur = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.id_utilisateur);
                });

            migrationBuilder.CreateTable(
                name: "Coffre",
                columns: table => new
                {
                    id_coffre = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    hash_masterkey = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    salt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    id_utilisateur = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coffre", x => x.id_coffre);
                    table.ForeignKey(
                        name: "FK_Coffre_Utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id_utilisateur");
                });

            migrationBuilder.CreateTable(
                name: "Dossier",
                columns: table => new
                {
                    Id_dossier = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    id_coffre = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dossier", x => x.Id_dossier);
                    table.ForeignKey(
                        name: "FK_Dossier_Coffre_id_coffre",
                        column: x => x.id_coffre,
                        principalTable: "Coffre",
                        principalColumn: "id_coffre");
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    id_log = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    texte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    timestamp_ = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_coffre = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.id_log);
                    table.ForeignKey(
                        name: "FK_Log_Coffre_id_coffre",
                        column: x => x.id_coffre,
                        principalTable: "Coffre",
                        principalColumn: "id_coffre");
                });

            migrationBuilder.CreateTable(
                name: "Partage",
                columns: table => new
                {
                    id_utilisateur = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    id_coffre = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partage", x => new { x.id_utilisateur, x.id_coffre });
                    table.ForeignKey(
                        name: "FK_Partage_Coffre_id_coffre",
                        column: x => x.id_coffre,
                        principalTable: "Coffre",
                        principalColumn: "id_coffre");
                    table.ForeignKey(
                        name: "FK_Partage_Utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id_utilisateur");
                });

            migrationBuilder.CreateTable(
                name: "Entree",
                columns: table => new
                {
                    id_entree = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    date_creation = table.Column<DateTime>(type: "datetime", nullable: false),
                    id_dossier = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entree", x => x.id_entree);
                    table.ForeignKey(
                        name: "FK_Entree_Dossier_id_dossier",
                        column: x => x.id_dossier,
                        principalTable: "Dossier",
                        principalColumn: "Id_dossier");
                });

            migrationBuilder.CreateTable(
                name: "Historique_entree",
                columns: table => new
                {
                    id_historique = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    libelle = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    libelle_tag = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    libelle_vi = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    date_update = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false),
                    username = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    username_tag = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    username_vi = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    url = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    url_tag = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    url_vi = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    password = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    password_tag = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    password_vi = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    commentaire = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    commentaire_tag = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    commentaire_vi = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: false),
                    id_entree = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historique_entree", x => x.id_historique);
                    table.ForeignKey(
                        name: "FK_Historique_entree_Entree_id_entree",
                        column: x => x.id_entree,
                        principalTable: "Entree",
                        principalColumn: "id_entree");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coffre_id_utilisateur",
                table: "Coffre",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Dossier_id_coffre",
                table: "Dossier",
                column: "id_coffre");

            migrationBuilder.CreateIndex(
                name: "IX_Entree_id_dossier",
                table: "Entree",
                column: "id_dossier");

            migrationBuilder.CreateIndex(
                name: "IX_Historique_entree_id_entree",
                table: "Historique_entree",
                column: "id_entree");

            migrationBuilder.CreateIndex(
                name: "IX_Log_id_coffre",
                table: "Log",
                column: "id_coffre");

            migrationBuilder.CreateIndex(
                name: "IX_Partage_id_coffre",
                table: "Partage",
                column: "id_coffre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Historique_entree");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Partage");

            migrationBuilder.DropTable(
                name: "Entree");

            migrationBuilder.DropTable(
                name: "Dossier");

            migrationBuilder.DropTable(
                name: "Coffre");

            migrationBuilder.DropTable(
                name: "Utilisateur");
        }
    }
}
