using Microsoft.EntityFrameworkCore.Migrations;

namespace LOL_int_list_GUI_v2.Migrations
{
    public partial class LockFileTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LockFileLocation",
                columns: table => new
                {
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockFileLocation", x => x.FilePath);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LockFileLocation");
        }
    }
}
