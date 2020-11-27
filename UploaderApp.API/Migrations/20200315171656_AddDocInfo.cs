using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    SalesforceId = table.Column<string>(nullable: true),
                    DocumentFullName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UniqueLinkId = table.Column<Guid>(nullable: false),
                    dateSent = table.Column<DateTime>(nullable: false),
                    dateViewed = table.Column<DateTime>(nullable: false),
                    dateAgreed = table.Column<DateTime>(nullable: false),
                    dateConfirmationSent = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocInfoId", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyProperty");
        }
    }
}
