using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FiapEsperancaSolidaria.Usuario.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Roles",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "User",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                FirebaseUserId = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_User", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserRoles",
            columns: table => new
            {
                RoleId = table.Column<int>(type: "integer", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRoles", x => new { x.RoleId, x.UserId });
                table.ForeignKey(
                    name: "FK_UserRoles_Roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserRoles_User_UserId",
                    column: x => x.UserId,
                    principalTable: "User",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "Roles",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                    { 1, "Admin" },
                    { 2, "User" }
            });

        migrationBuilder.InsertData(
            table: "User",
            columns: new[] { "Id", "Name", "Email", "FirebaseUserId" },
            values: new object[,]
            {
                    { "29b77427-d0f9-4e02-a281-a825e73a9204", "Admin1", "admin1@fiapcloudgames.com", "7h3ep3StXbhkn0fXwbtugBYMmvI3"}
            });
        migrationBuilder.InsertData(
            table: "UserRoles",
            columns: new[] { "RoleId", "UserId" },
            values: new object[,]
            {
                    { 1, "29b77427-d0f9-4e02-a281-a825e73a9204"}
            });


        migrationBuilder.CreateIndex(
            name: "IX_UserRoles_UserId",
            table: "UserRoles",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserRoles");

        migrationBuilder.DropTable(
            name: "Roles");

        migrationBuilder.DropTable(
            name: "User");
    }
}