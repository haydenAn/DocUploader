using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace UploaderApp.API.Migrations
{
    public partial class AddDocInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    LastName = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    EmailAddress = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    Title = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    Company = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    SalesforceId = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    DocumentFullName = table.Column<string>(type: "VARCHAR(300)", nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UniqueLinkId = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    dateSent = table.Column<DateTime>(nullable: false),
                    dateViewed = table.Column<DateTime>(nullable: false),
                    dateAgreed = table.Column<DateTime>(nullable: false),
                    dateResent = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentInfo");

            migrationBuilder.DropTable(
                name: "Values");
        }
    }
}
