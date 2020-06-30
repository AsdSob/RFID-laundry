using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Laundry.Migrations
{
    public partial class NameInStaffDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "clientStaff",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "clientStaff");
        }
    }
}
