using Microsoft.EntityFrameworkCore.Migrations;

namespace Arshinov.WebApp.Migrations
{
    public partial class initfix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacteristicType1",
                table: "Characteristics");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CharacteristicType1",
                table: "Characteristics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
