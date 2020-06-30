using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Laundry.Migrations
{
    public partial class changingStaffStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clientLinen_clientStaff_staffId",
                table: "clientLinen");

            migrationBuilder.DropIndex(
                name: "IX_clientStaff_departmentId",
                table: "clientStaff");

            migrationBuilder.DropIndex(
                name: "IX_clientLinen_staffId",
                table: "clientLinen");

            migrationBuilder.DropColumn(
                name: "name",
                table: "clientStaff");

            migrationBuilder.DropColumn(
                name: "staffId",
                table: "clientLinen");

            migrationBuilder.AlterColumn<int>(
                name: "departmentTypeId",
                table: "department",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "parentId",
                table: "department",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientStaff_departmentId",
                table: "clientStaff",
                column: "departmentId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clientStaff_departmentId",
                table: "clientStaff");

            migrationBuilder.DropColumn(
                name: "parentId",
                table: "department");

            migrationBuilder.AlterColumn<int>(
                name: "departmentTypeId",
                table: "department",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "clientStaff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "staffId",
                table: "clientLinen",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientStaff_departmentId",
                table: "clientStaff",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_clientLinen_staffId",
                table: "clientLinen",
                column: "staffId");

            migrationBuilder.AddForeignKey(
                name: "FK_clientLinen_clientStaff_staffId",
                table: "clientLinen",
                column: "staffId",
                principalTable: "clientStaff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
