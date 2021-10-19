using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class FinalCustomerForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DODAffiliationID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceStatusID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DODAffiliationID",
                table: "AspNetUsers",
                column: "DODAffiliationID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ServiceStatusID",
                table: "AspNetUsers",
                column: "ServiceStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_DOD_Affiliation_DODAffiliationID",
                table: "AspNetUsers",
                column: "DODAffiliationID",
                principalTable: "DOD_Affiliation",
                principalColumn: "DODAffiliationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Service_Status_Type_ServiceStatusID",
                table: "AspNetUsers",
                column: "ServiceStatusID",
                principalTable: "Service_Status_Type",
                principalColumn: "ServiceStatusID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_DOD_Affiliation_DODAffiliationID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Service_Status_Type_ServiceStatusID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DODAffiliationID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ServiceStatusID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DODAffiliationID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ServiceStatusID",
                table: "AspNetUsers");
        }
    }
}
