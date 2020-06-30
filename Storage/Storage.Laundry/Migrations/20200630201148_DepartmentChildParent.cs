using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Laundry.Migrations
{
    public partial class DepartmentChildParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_department_parentId",
                table: "department",
                column: "parentId");

            migrationBuilder.AddForeignKey(
                name: "FK_department_department_parentId",
                table: "department",
                column: "parentId",
                principalTable: "department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_department_department_parentId",
                table: "department");

            migrationBuilder.DropIndex(
                name: "IX_department_parentId",
                table: "department");
        }
    }
}
