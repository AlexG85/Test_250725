using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Examen.Migrations
{
    /// <inheritdoc />
    public partial class employeeContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKEmployee_EmployeeId", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "tModules",
                keyColumn: "ModuleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 13, 22, 41, 368, DateTimeKind.Local).AddTicks(548));

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FirstName",
                table: "tEmployees",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LastName",
                table: "tEmployees",
                column: "LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tEmployees");

            migrationBuilder.UpdateData(
                table: "tModules",
                keyColumn: "ModuleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 3, 17, 53, 36, 927, DateTimeKind.Local).AddTicks(3231));
        }
    }
}
