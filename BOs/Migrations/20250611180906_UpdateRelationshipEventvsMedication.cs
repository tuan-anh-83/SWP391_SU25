using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationshipEventvsMedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalEvent_Medication_MedicationId",
                table: "MedicalEvent");

            migrationBuilder.DropIndex(
                name: "IX_MedicalEvent_MedicationId",
                table: "MedicalEvent");

            migrationBuilder.CreateTable(
                name: "MedicalEventMedication",
                columns: table => new
                {
                    MedicalEventsMedicalEventId = table.Column<int>(type: "int", nullable: false),
                    MedicationsMedicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalEventMedication", x => new { x.MedicalEventsMedicalEventId, x.MedicationsMedicationId });
                    table.ForeignKey(
                        name: "FK_MedicalEventMedication_MedicalEvent_MedicalEventsMedicalEventId",
                        column: x => x.MedicalEventsMedicalEventId,
                        principalTable: "MedicalEvent",
                        principalColumn: "MedicalEventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalEventMedication_Medication_MedicationsMedicationId",
                        column: x => x.MedicationsMedicationId,
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalEventMedication_MedicationsMedicationId",
                table: "MedicalEventMedication",
                column: "MedicationsMedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalEventMedication");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalEvent_MedicationId",
                table: "MedicalEvent",
                column: "MedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalEvent_Medication_MedicationId",
                table: "MedicalEvent",
                column: "MedicationId",
                principalTable: "Medication",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
