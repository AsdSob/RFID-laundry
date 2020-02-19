using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Storage.Laundry.Migrations
{
    public partial class Migration_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    parentId = table.Column<int>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    shortName = table.Column<string>(nullable: true),
                    active = table.Column<bool>(nullable: false),
                    address = table.Column<string>(nullable: true),
                    cityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client", x => x.Id);
                    table.ForeignKey(
                        name: "FK_client_client_parentId",
                        column: x => x.parentId,
                        principalTable: "client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "laundry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laundry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "masterLinen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    packingValue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_masterLinen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rfidReader",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    readerIp = table.Column<string>(nullable: true),
                    readerPort = table.Column<int>(nullable: false),
                    tagPopulation = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfidReader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    clientId = table.Column<int>(nullable: false),
                    departmentTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_department_client_clientId",
                        column: x => x.clientId,
                        principalTable: "client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rfidAntenna",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    antennaNumb = table.Column<int>(nullable: false),
                    rfidReaderId = table.Column<int>(nullable: false),
                    rxSensitivity = table.Column<double>(nullable: false),
                    txPower = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfidAntenna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rfidAntenna_rfidReader_rfidReaderId",
                        column: x => x.rfidReaderId,
                        principalTable: "rfidReader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clientStaff",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    staffId = table.Column<string>(nullable: true),
                    departmentId = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    phoneNumber = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_clientStaff_department_departmentId",
                        column: x => x.departmentId,
                        principalTable: "department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clientLinen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    departmentId = table.Column<int>(nullable: false),
                    clientId = table.Column<int>(nullable: false),
                    masterLinenId = table.Column<int>(nullable: false),
                    staffId = table.Column<int>(nullable: true),
                    rfidTag = table.Column<string>(nullable: true),
                    statusId = table.Column<int>(nullable: false),
                    packingValue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientLinen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_clientLinen_client_clientId",
                        column: x => x.clientId,
                        principalTable: "client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clientLinen_department_departmentId",
                        column: x => x.departmentId,
                        principalTable: "department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clientLinen_masterLinen_masterLinenId",
                        column: x => x.masterLinenId,
                        principalTable: "masterLinen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clientLinen_clientStaff_staffId",
                        column: x => x.staffId,
                        principalTable: "clientStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "conveyor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    beltNumber = table.Column<int>(nullable: false),
                    slotNumber = table.Column<int>(nullable: false),
                    clientLinenId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conveyor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_conveyor_clientLinen_clientLinenId",
                        column: x => x.clientLinenId,
                        principalTable: "clientLinen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_client_parentId",
                table: "client",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_clientLinen_clientId",
                table: "clientLinen",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_clientLinen_departmentId",
                table: "clientLinen",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_clientLinen_masterLinenId",
                table: "clientLinen",
                column: "masterLinenId");

            migrationBuilder.CreateIndex(
                name: "IX_clientLinen_staffId",
                table: "clientLinen",
                column: "staffId");

            migrationBuilder.CreateIndex(
                name: "IX_clientStaff_departmentId",
                table: "clientStaff",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_conveyor_clientLinenId",
                table: "conveyor",
                column: "clientLinenId");

            migrationBuilder.CreateIndex(
                name: "IX_department_clientId",
                table: "department",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_rfidAntenna_rfidReaderId",
                table: "rfidAntenna",
                column: "rfidReaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conveyor");

            migrationBuilder.DropTable(
                name: "laundry");

            migrationBuilder.DropTable(
                name: "rfidAntenna");

            migrationBuilder.DropTable(
                name: "clientLinen");

            migrationBuilder.DropTable(
                name: "rfidReader");

            migrationBuilder.DropTable(
                name: "masterLinen");

            migrationBuilder.DropTable(
                name: "clientStaff");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "client");
        }
    }
}
