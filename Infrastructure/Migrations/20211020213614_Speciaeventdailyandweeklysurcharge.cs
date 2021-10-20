using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class Speciaeventdailyandweeklysurcharge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Daily_Surcharge",
                table: "Special_Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Weekly_Surcharge",
                table: "Special_Event",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Daily_Surcharge",
                table: "Special_Event");

            migrationBuilder.DropColumn(
                name: "Weekly_Surcharge",
                table: "Special_Event");
        }
    }
}
