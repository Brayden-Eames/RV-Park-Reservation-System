using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class InitialCustomerTBL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientResponse");

            migrationBuilder.DropTable(
                name: "FriendResponse");

            migrationBuilder.DropTable(
                name: "Adjective");

            migrationBuilder.DropTable(
                name: "Friend");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "AspNetUsers",
                newName: "CustPhone");

            migrationBuilder.RenameColumn(
                name: "dateOfBirth",
                table: "AspNetUsers",
                newName: "CustLastModifiedDate");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "AspNetUsers",
                newName: "CustLastName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "CustLastModifiedBy");

            migrationBuilder.AddColumn<string>(
                name: "CustEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustFirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustFirstName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CustPhone",
                table: "AspNetUsers",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "CustLastName",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "CustLastModifiedDate",
                table: "AspNetUsers",
                newName: "dateOfBirth");

            migrationBuilder.RenameColumn(
                name: "CustLastModifiedBy",
                table: "AspNetUsers",
                newName: "FirstName");

            migrationBuilder.CreateTable(
                name: "Adjective",
                columns: table => new
                {
                    AdjectiveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdjName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdjType = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjective", x => x.AdjectiveId);
                });

            migrationBuilder.CreateTable(
                name: "Friend",
                columns: table => new
                {
                    FriendId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    howLong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friend", x => x.FriendId);
                });

            migrationBuilder.CreateTable(
                name: "ClientResponse",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjectiveId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientResponse", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_ClientResponse_Adjective_AdjectiveId",
                        column: x => x.AdjectiveId,
                        principalTable: "Adjective",
                        principalColumn: "AdjectiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientResponse_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FriendResponse",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjectiveId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FriendId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendResponse", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_FriendResponse_Adjective_AdjectiveId",
                        column: x => x.AdjectiveId,
                        principalTable: "Adjective",
                        principalColumn: "AdjectiveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendResponse_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendResponse_Friend_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Friend",
                        principalColumn: "FriendId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientResponse_AdjectiveId",
                table: "ClientResponse",
                column: "AdjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientResponse_ClientId",
                table: "ClientResponse",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendResponse_AdjectiveId",
                table: "FriendResponse",
                column: "AdjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendResponse_ClientId",
                table: "FriendResponse",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendResponse_FriendId",
                table: "FriendResponse",
                column: "FriendId");
        }
    }
}
