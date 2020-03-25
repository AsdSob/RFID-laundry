using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Storage.Laundry.Migrations
{
    public partial class Migration_3_accountDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accountDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    accountId = table.Column<int>(nullable: false),
                    readerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_accountDetails_account_accountId",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_accountDetails_rfidReader_readerId",
                        column: x => x.readerId,
                        principalTable: "rfidReader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accountDetails_accountId",
                table: "accountDetails",
                column: "accountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accountDetails_readerId",
                table: "accountDetails",
                column: "readerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accountDetails");
        }
    }
}
