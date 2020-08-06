using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Laundry.Migrations
{
    public partial class addingStaffIdInClientLinen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "staffId",
                table: "clientLinen",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "staffId",
                table: "clientLinen");
        }
    }
}
