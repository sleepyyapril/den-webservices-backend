using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DenWebServices.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignedUserIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedUserIds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RankFlag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Flag = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RankId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankFlag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankFlag_Ranks_RankId",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    RankId = table.Column<int>(type: "integer", nullable: true),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DiscordId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_UniqueId", x => x.UniqueId);
                    table.ForeignKey(
                        name: "FK_Users_Ranks_RankId",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Suspended = table.Column<bool>(type: "boolean", nullable: false),
                    AdminRankId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Ranks_AdminRankId",
                        column: x => x.AdminRankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminFlag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Flag = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Negative = table.Column<bool>(type: "boolean", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminFlag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminFlag_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Punishments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PunishingUserUniqueId = table.Column<Guid>(type: "uuid", nullable: true),
                    PunishingAdminId = table.Column<int>(type: "integer", nullable: false),
                    PunishmentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Start = table.Column<long>(type: "bigint", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Punishments_Admins_PunishingAdminId",
                        column: x => x.PunishingAdminId,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Punishments_Users_PunishingUserUniqueId",
                        column: x => x.PunishingUserUniqueId,
                        principalTable: "Users",
                        principalColumn: "UniqueId");
                    table.ForeignKey(
                        name: "FK_Punishments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminFlag_AdminId",
                table: "AdminFlag",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminFlag_Flag_AdminId",
                table: "AdminFlag",
                columns: new[] { "Flag", "AdminId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_AdminRankId",
                table: "Admins",
                column: "AdminRankId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedUserIds_UserId",
                table: "AssignedUserIds",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignedUserIds_UserName",
                table: "AssignedUserIds",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_PunishingAdminId",
                table: "Punishments",
                column: "PunishingAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_PunishingUserUniqueId",
                table: "Punishments",
                column: "PunishingUserUniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Punishments_UserId",
                table: "Punishments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RankFlag_Flag_RankId",
                table: "RankFlag",
                columns: new[] { "Flag", "RankId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RankFlag_RankId",
                table: "RankFlag",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RankId",
                table: "Users",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UniqueId",
                table: "Users",
                column: "UniqueId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminFlag");

            migrationBuilder.DropTable(
                name: "AssignedUserIds");

            migrationBuilder.DropTable(
                name: "Punishments");

            migrationBuilder.DropTable(
                name: "RankFlag");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Ranks");
        }
    }
}
