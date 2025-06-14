using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "MedicalEvent",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateTable(
                name: "MedicalSupply",
                columns: table => new
                {
                    MedicalSupplyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalSupply", x => x.MedicalSupplyId);
                });

            migrationBuilder.CreateTable(
                name: "MedicalEventMedicalSupply",
                columns: table => new
                {
                    MedicalEventsMedicalEventId = table.Column<int>(type: "int", nullable: false),
                    MedicalSuppliesMedicalSupplyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalEventMedicalSupply", x => new { x.MedicalEventsMedicalEventId, x.MedicalSuppliesMedicalSupplyId });
                    table.ForeignKey(
                        name: "FK_MedicalEventMedicalSupply_MedicalEvent_MedicalEventsMedicalEventId",
                        column: x => x.MedicalEventsMedicalEventId,
                        principalTable: "MedicalEvent",
                        principalColumn: "MedicalEventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalEventMedicalSupply_MedicalSupply_MedicalSuppliesMedicalSupplyId",
                        column: x => x.MedicalSuppliesMedicalSupplyId,
                        principalTable: "MedicalSupply",
                        principalColumn: "MedicalSupplyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalEventMedicalSupply_MedicalSuppliesMedicalSupplyId",
                table: "MedicalEventMedicalSupply",
                column: "MedicalSuppliesMedicalSupplyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalEventMedicalSupply");

            migrationBuilder.DropTable(
                name: "MedicalSupply");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "MedicalEvent",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
