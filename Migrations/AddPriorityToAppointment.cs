using Microsoft.EntityFrameworkCore.Migrations;

namespace AppointmentApi.Migrations
{
    public partial class UpdatePriorityToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First drop the existing column if it exists
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Appointments");

            // Add it back as an integer with default value 0 (low)
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert back to string
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Appointments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "low");
        }
    }
}
