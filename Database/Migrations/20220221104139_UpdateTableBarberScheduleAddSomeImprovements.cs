using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    public partial class UpdateTableBarberScheduleAddSomeImprovements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntervalTime",
                table: "BarberSchedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndInterval",
                table: "BarberSchedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartInterval",
                table: "BarberSchedules",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndInterval",
                table: "BarberSchedules");

            migrationBuilder.DropColumn(
                name: "StartInterval",
                table: "BarberSchedules");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "IntervalTime",
                table: "BarberSchedules",
                type: "interval",
                nullable: true);
        }
    }
}
