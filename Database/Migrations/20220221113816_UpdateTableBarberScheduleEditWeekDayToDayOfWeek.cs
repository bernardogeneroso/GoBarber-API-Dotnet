using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    public partial class UpdateTableBarberScheduleEditWeekDayToDayOfWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weekday",
                table: "BarberSchedules",
                newName: "DayOfWeek");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DayOfWeek",
                table: "BarberSchedules",
                newName: "Weekday");
        }
    }
}
