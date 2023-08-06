using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Epub3Editor.Database.Migrations
{
    public partial class AddBlockName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Blocks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Blocks");
        }
    }
}
