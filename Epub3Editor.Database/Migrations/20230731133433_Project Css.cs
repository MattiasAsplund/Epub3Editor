using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Epub3Editor.Database.Migrations
{
    public partial class ProjectCss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Css",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Css",
                table: "Projects");
        }
    }
}
