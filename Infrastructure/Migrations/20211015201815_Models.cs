using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class Models : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer_Password",
                columns: table => new
                {
                    CustPassID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Password = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    PasswordAssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer_Password", x => x.CustPassID);
                    table.ForeignKey(
                        name: "FK_Customer_Password_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DOD_Affiliation",
                columns: table => new
                {
                    DODAffiliationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DODAffiliationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOD_Affiliation", x => x.DODAffiliationID);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationAddress1 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LocationAddress2 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LocationCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationState = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LocationZip = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "Payment_Reason",
                columns: table => new
                {
                    PayReasonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayReasonName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Reason", x => x.PayReasonID);
                });

            migrationBuilder.CreateTable(
                name: "Payment_Type",
                columns: table => new
                {
                    PayTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayType = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Type", x => x.PayTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Reservation_Status",
                columns: table => new
                {
                    ResStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResStatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResStatusDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation_Status", x => x.ResStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Security_Question",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Security_Question", x => x.QuestionID);
                });

            migrationBuilder.CreateTable(
                name: "Service_Status_Type",
                columns: table => new
                {
                    ServiceStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceStatusType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service_Status_Type", x => x.ServiceStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle_Type",
                columns: table => new
                {
                    TypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle_Type", x => x.TypeID);
                });

            migrationBuilder.CreateTable(
                name: "Site_Category",
                columns: table => new
                {
                    SiteCategory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteCategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteCategoryDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site_Category", x => x.SiteCategory);
                    table.ForeignKey(
                        name: "FK_Site_Category_Location_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Special_Event",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Special_Event", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_Special_Event_Location_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Security_Answer",
                columns: table => new
                {
                    Answer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Security_Answer", x => x.Answer);
                    table.ForeignKey(
                        name: "FK_Security_Answer_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Security_Answer_Security_Question_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Security_Question",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    SiteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteNumber = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    SiteLength = table.Column<int>(type: "int", nullable: false),
                    SiteDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteLastModifiedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SiteLastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SiteCategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.SiteID);
                    table.ForeignKey(
                        name: "FK_Site_Site_Category_SiteCategoryID",
                        column: x => x.SiteCategoryID,
                        principalTable: "Site_Category",
                        principalColumn: "SiteCategory",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Site_Rate",
                columns: table => new
                {
                    RateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RateAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    RateStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RateEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RateLastModifiedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RateModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SiteCategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site_Rate", x => x.RateID);
                    table.ForeignKey(
                        name: "FK_Site_Rate_Site_Category_SiteCategoryID",
                        column: x => x.SiteCategoryID,
                        principalTable: "Site_Category",
                        principalColumn: "SiteCategory",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    ResID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResNumAdults = table.Column<int>(type: "int", nullable: false),
                    ResNumChildren = table.Column<int>(type: "int", nullable: false),
                    ResNumPets = table.Column<int>(type: "int", nullable: false),
                    ResAcknowledgeValidPets = table.Column<bool>(type: "bit", nullable: false),
                    ResStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResVehicleLength = table.Column<int>(type: "int", nullable: false),
                    ResLastModifiedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ResLastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleTypeID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    ResStatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.ResID);
                    table.ForeignKey(
                        name: "FK_Reservation_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_Reservation_Status_ResStatusID",
                        column: x => x.ResStatusID,
                        principalTable: "Reservation_Status",
                        principalColumn: "ResStatusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_Site_SiteID",
                        column: x => x.SiteID,
                        principalTable: "Site",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_Vehicle_Type_TypeID",
                        column: x => x.TypeID,
                        principalTable: "Vehicle_Type",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayTotalCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CCReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PayLastModifiedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PayLastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResID = table.Column<int>(type: "int", nullable: false),
                    PayReasonID = table.Column<int>(type: "int", nullable: false),
                    PayTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PayID);
                    table.ForeignKey(
                        name: "FK_Payment_Payment_Reason_PayReasonID",
                        column: x => x.PayReasonID,
                        principalTable: "Payment_Reason",
                        principalColumn: "PayReasonID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Payment_Type_PayTypeID",
                        column: x => x.PayTypeID,
                        principalTable: "Payment_Type",
                        principalColumn: "PayTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Reservation_ResID",
                        column: x => x.ResID,
                        principalTable: "Reservation",
                        principalColumn: "ResID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Password_Id",
                table: "Customer_Password",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PayReasonID",
                table: "Payment",
                column: "PayReasonID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PayTypeID",
                table: "Payment",
                column: "PayTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ResID",
                table: "Payment",
                column: "ResID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_Id",
                table: "Reservation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_ResStatusID",
                table: "Reservation",
                column: "ResStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_SiteID",
                table: "Reservation",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_TypeID",
                table: "Reservation",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Security_Answer_Id",
                table: "Security_Answer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Security_Answer_QuestionID",
                table: "Security_Answer",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_Site_SiteCategoryID",
                table: "Site",
                column: "SiteCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Site_Category_LocationID",
                table: "Site_Category",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Site_Rate_SiteCategoryID",
                table: "Site_Rate",
                column: "SiteCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Special_Event_LocationID",
                table: "Special_Event",
                column: "LocationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer_Password");

            migrationBuilder.DropTable(
                name: "DOD_Affiliation");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Security_Answer");

            migrationBuilder.DropTable(
                name: "Service_Status_Type");

            migrationBuilder.DropTable(
                name: "Site_Rate");

            migrationBuilder.DropTable(
                name: "Special_Event");

            migrationBuilder.DropTable(
                name: "Payment_Reason");

            migrationBuilder.DropTable(
                name: "Payment_Type");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Security_Question");

            migrationBuilder.DropTable(
                name: "Reservation_Status");

            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.DropTable(
                name: "Vehicle_Type");

            migrationBuilder.DropTable(
                name: "Site_Category");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
