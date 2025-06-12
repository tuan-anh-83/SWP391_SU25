using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class RequestSendMedication2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Note",
                table: "ParentMedicationRequest",
                newName: "ParentNote");

            migrationBuilder.AddColumn<string>(
                name: "NurseNote",
                table: "ParentMedicationRequest",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NurseNote",
                table: "ParentMedicationRequest");

            migrationBuilder.RenameColumn(
                name: "ParentNote",
                table: "ParentMedicationRequest",
                newName: "Note");
        }
    }
}
