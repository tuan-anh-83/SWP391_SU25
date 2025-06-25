using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class addEyes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NutritionStatus",
                table: "HealthRecord");

            migrationBuilder.DropColumn(
                name: "NutritionStatus",
                table: "HealthCheck");

            migrationBuilder.RenameColumn(
                name: "BMI",
                table: "HealthRecord",
                newName: "RightEye");

            migrationBuilder.RenameColumn(
                name: "BMI",
                table: "HealthCheck",
                newName: "RightEye");

            migrationBuilder.AddColumn<double>(
                name: "LeftEye",
                table: "HealthRecord",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LeftEye",
                table: "HealthCheck",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftEye",
                table: "HealthRecord");

            migrationBuilder.DropColumn(
                name: "LeftEye",
                table: "HealthCheck");

            migrationBuilder.RenameColumn(
                name: "RightEye",
                table: "HealthRecord",
                newName: "BMI");

            migrationBuilder.RenameColumn(
                name: "RightEye",
                table: "HealthCheck",
                newName: "BMI");

            migrationBuilder.AddColumn<string>(
                name: "NutritionStatus",
                table: "HealthRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NutritionStatus",
                table: "HealthCheck",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
