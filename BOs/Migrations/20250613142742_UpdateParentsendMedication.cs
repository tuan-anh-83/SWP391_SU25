using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateParentsendMedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParentMedicationRequestMedication");

            migrationBuilder.AlterColumn<string>(
                name: "ParentNote",
                table: "ParentMedicationRequest",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "NurseNote",
                table: "ParentMedicationRequest",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateTable(
                name: "ParentMedicationDetail",
                columns: table => new
                {
                    MedicationDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Usage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentMedicationDetail", x => x.MedicationDetailId);
                    table.ForeignKey(
                        name: "FK_ParentMedicationDetail_ParentMedicationRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ParentMedicationRequest",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParentMedicationDetail_RequestId",
                table: "ParentMedicationDetail",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParentMedicationDetail");

            migrationBuilder.AlterColumn<string>(
                name: "ParentNote",
                table: "ParentMedicationRequest",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NurseNote",
                table: "ParentMedicationRequest",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

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
                name: "IX_ParentMedicationRequestMedication_ParentMedicationRequestRequestId",
                table: "ParentMedicationRequestMedication",
                column: "ParentMedicationRequestRequestId");
        }
    }
}
