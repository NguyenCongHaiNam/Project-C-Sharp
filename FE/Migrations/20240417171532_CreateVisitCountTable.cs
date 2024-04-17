using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FE.Migrations
{
    public partial class CreateVisitCountTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "label_data",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    label = table.Column<string>(fixedLength: true, maxLength: 20, nullable: false),
                    content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    logID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUser = table.Column<int>(nullable: false),
                    logContent = table.Column<string>(nullable: false),
                    dateTime = table.Column<DateTime>(fixedLength: true, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Log__7839F62D023AD65F", x => x.logID);
                });

            migrationBuilder.CreateTable(
                name: "raw",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    url = table.Column<string>(fixedLength: true, maxLength: 1000, nullable: false),
                    title = table.Column<string>(fixedLength: true, maxLength: 500, nullable: false),
                    keywords = table.Column<string>(fixedLength: true, maxLength: 500, nullable: false),
                    published_date = table.Column<string>(fixedLength: true, maxLength: 100, nullable: false),
                    top_img = table.Column<string>(fixedLength: true, maxLength: 300, nullable: false),
                    content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raw", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    idUser = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    LastName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3717C98294906179", x => x.idUser);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "label_data");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "raw");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
