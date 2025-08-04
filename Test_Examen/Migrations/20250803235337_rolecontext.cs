using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Examen.Migrations
{
    /// <inheritdoc />
    public partial class rolecontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "app_tUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tModules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKModule_ModuleId", x => x.ModuleId);
                });

            migrationBuilder.CreateTable(
                name: "tRoles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKRole_RoleId", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "tPermissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    ModuleType = table.Column<int>(type: "int", nullable: false),
                    PermissionType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKPermission_PermissionId", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_tPermissions_tModules_ModuleType",
                        column: x => x.ModuleType,
                        principalTable: "tModules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rRolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKRolePermission_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_rRolePermissions_tPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "tPermissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rRolePermissions_tRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tModules",
                columns: new[] { "ModuleId", "CreatedAt", "CreatedBy", "Description", "IsActive", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[] { 1, new DateTime(2025, 8, 3, 17, 53, 36, 927, DateTimeKind.Local).AddTicks(3231), "", "General Module", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "General" });

            migrationBuilder.InsertData(
                table: "tRoles",
                columns: new[] { "RoleId", "Description", "IsActive" },
                values: new object[] { 1, "DefaultRole", true });

            migrationBuilder.CreateIndex(
                name: "IX_app_tUsers_RoleId",
                table: "app_tUsers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_rRolePermissions_PermissionId",
                table: "rRolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_ModulePermission",
                table: "tPermissions",
                columns: new[] { "ModuleType", "PermissionType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_app_tUsers_tRoles_RoleId",
                table: "app_tUsers",
                column: "RoleId",
                principalTable: "tRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_app_tUsers_tRoles_RoleId",
                table: "app_tUsers");

            migrationBuilder.DropTable(
                name: "rRolePermissions");

            migrationBuilder.DropTable(
                name: "tPermissions");

            migrationBuilder.DropTable(
                name: "tRoles");

            migrationBuilder.DropTable(
                name: "tModules");

            migrationBuilder.DropIndex(
                name: "IX_app_tUsers_RoleId",
                table: "app_tUsers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "app_tUsers");
        }
    }
}
