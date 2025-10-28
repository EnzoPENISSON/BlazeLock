using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeLock.API.Migrations
{
    /// <inheritdoc />
    public partial class ajoutguid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dossier",
                columns: table => new
                {
                    Id_dossier = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Id_dossier_1 = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dossier__4484C37EEDA5F640", x => x.Id_dossier);
                    table.ForeignKey(
                        name: "FK__Dossier__Id_doss__3C69FB99",
                        column: x => x.Id_dossier_1,
                        principalTable: "Dossier",
                        principalColumn: "Id_dossier");
                });

            migrationBuilder.CreateTable(
                name: "Entree",
                columns: table => new
                {
                    id_entree = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    date_creation = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Entree__B8AEB9F2EB302244", x => x.id_entree);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    id_utilisateur = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Utilisat__1A4FA5B84005D655", x => x.id_utilisateur);
                });

            migrationBuilder.CreateTable(
                name: "Historique_entree",
                columns: table => new
                {
                    id_historique = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    libelle_tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    libelle_vi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    date_update = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    username_tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    username_vi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    url = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    url_tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    url_vi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_vi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    commentaire = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    commentaire_tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    commentaire_vi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    id_entree = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Historiq__33F46ECB49A6FF51", x => x.id_historique);
                    table.ForeignKey(
                        name: "FK__Historiqu__id_en__412EB0B6",
                        column: x => x.id_entree,
                        principalTable: "Entree",
                        principalColumn: "id_entree");
                });

            migrationBuilder.CreateTable(
                name: "Stocker",
                columns: table => new
                {
                    Id_dossier = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    id_entree = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Stocker__7F0E28E12110E1D5", x => new { x.Id_dossier, x.id_entree });
                    table.ForeignKey(
                        name: "FK__Stocker__Id_doss__4E88ABD4",
                        column: x => x.Id_dossier,
                        principalTable: "Dossier",
                        principalColumn: "Id_dossier");
                    table.ForeignKey(
                        name: "FK__Stocker__id_entr__4F7CD00D",
                        column: x => x.id_entree,
                        principalTable: "Entree",
                        principalColumn: "id_entree");
                });

            migrationBuilder.CreateTable(
                name: "Coffre",
                columns: table => new
                {
                    id_coffre = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    hash_masterkey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    salt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    id_utilisateur = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Coffre__47508D12F46D3B31", x => x.id_coffre);
                    table.ForeignKey(
                        name: "FK__Coffre__id_utili__398D8EEE",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id_utilisateur");
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    id_log = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    texte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    timestamp_ = table.Column<DateTime>(type: "datetime", nullable: true),
                    id_coffre = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Log__6CC851FE0CAB883A", x => x.id_log);
                    table.ForeignKey(
                        name: "FK__Log__id_coffre__440B1D61",
                        column: x => x.id_coffre,
                        principalTable: "Coffre",
                        principalColumn: "id_coffre");
                });

            migrationBuilder.CreateTable(
                name: "Organiser",
                columns: table => new
                {
                    id_coffre = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    Id_dossier = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Organise__B318C125E234F68B", x => new { x.id_coffre, x.Id_dossier });
                    table.ForeignKey(
                        name: "FK__Organiser__Id_do__4BAC3F29",
                        column: x => x.Id_dossier,
                        principalTable: "Dossier",
                        principalColumn: "Id_dossier");
                    table.ForeignKey(
                        name: "FK__Organiser__id_co__4AB81AF0",
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
                    isAdmin = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Partage__2E3AAD6978ABB493", x => new { x.id_utilisateur, x.id_coffre });
                    table.ForeignKey(
                        name: "FK__Partage__id_coff__47DBAE45",
                        column: x => x.id_coffre,
                        principalTable: "Coffre",
                        principalColumn: "id_coffre");
                    table.ForeignKey(
                        name: "FK__Partage__id_util__46E78A0C",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id_utilisateur");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coffre_id_utilisateur",
                table: "Coffre",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Dossier_Id_dossier_1",
                table: "Dossier",
                column: "Id_dossier_1");

            migrationBuilder.CreateIndex(
                name: "IX_Historique_entree_id_entree",
                table: "Historique_entree",
                column: "id_entree");

            migrationBuilder.CreateIndex(
                name: "IX_Log_id_coffre",
                table: "Log",
                column: "id_coffre");

            migrationBuilder.CreateIndex(
                name: "IX_Organiser_Id_dossier",
                table: "Organiser",
                column: "Id_dossier");

            migrationBuilder.CreateIndex(
                name: "IX_Partage_id_coffre",
                table: "Partage",
                column: "id_coffre");

            migrationBuilder.CreateIndex(
                name: "IX_Stocker_id_entree",
                table: "Stocker",
                column: "id_entree");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Historique_entree");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Organiser");

            migrationBuilder.DropTable(
                name: "Partage");

            migrationBuilder.DropTable(
                name: "Stocker");

            migrationBuilder.DropTable(
                name: "Coffre");

            migrationBuilder.DropTable(
                name: "Dossier");

            migrationBuilder.DropTable(
                name: "Entree");

            migrationBuilder.DropTable(
                name: "Utilisateur");
        }
    }
}
