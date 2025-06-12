using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class RequestSendMedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParentMedicationRequest",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentMedicationRequest", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_ParentMedicationRequest_Account_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParentMedicationRequest_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentMedicationRequestMedication",
                columns: table => new
                {
                    MedicationsMedicationId = table.Column<int>(type: "int", nullable: false),
                    ParentMedicationRequestRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentMedicationRequestMedication", x => new { x.MedicationsMedicationId, x.ParentMedicationRequestRequestId });
                    table.ForeignKey(
                        name: "FK_ParentMedicationRequestMedication_Medication_MedicationsMedicationId",
                        column: x => x.MedicationsMedicationId,
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParentMedicationRequestMedication_ParentMedicationRequest_ParentMedicationRequestRequestId",
                        column: x => x.ParentMedicationRequestRequestId,
                        principalTable: "ParentMedicationRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParentMedicationRequest_ParentId",
                table: "ParentMedicationRequest",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentMedicationRequest_StudentId",
                table: "ParentMedicationRequest",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentMedicationRequestMedication_ParentMedicationRequestRequestId",
                table: "ParentMedicationRequestMedication",
                column: "ParentMedicationRequestRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParentMedicationRequestMedication");

            migrationBuilder.DropTable(
                name: "ParentMedicationRequest");
        }
    }
}
