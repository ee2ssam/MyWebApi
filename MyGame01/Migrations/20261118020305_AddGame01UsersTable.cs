using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGame01.Migrations
{
    /// <inheritdoc />
    public partial class AddGame01UsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game01_users",
                columns: table => new
                {
                    userid = table.Column<string>(type: "char(8)", maxLength: 8, nullable: false),
                    passward = table.Column<string>(type: "char(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    birthyear = table.Column<short>(type: "smallint", nullable: false),
                    addr = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    mobile = table.Column<string>(type: "char(11)", maxLength: 11, nullable: true),
                    level = table.Column<short>(type: "smallint", nullable: false),
                    health = table.Column<int>(type: "integer", nullable: false),
                    gold = table.Column<int>(type: "integer", nullable: false),
                    mdate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game01_users", x => x.userid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game01_users");
        }
    }
}
