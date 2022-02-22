using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    public partial class UpdateTableBarberSchedulesChangeUserIdToBarberId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarberSchedules_AspNetUsers_UserId",
                table: "BarberSchedules");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BarberSchedules",
                newName: "BarberId");

            migrationBuilder.RenameIndex(
                name: "IX_BarberSchedules_UserId",
                table: "BarberSchedules",
                newName: "IX_BarberSchedules_BarberId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarberSchedules_AspNetUsers_BarberId",
                table: "BarberSchedules",
                column: "BarberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarberSchedules_AspNetUsers_BarberId",
                table: "BarberSchedules");

            migrationBuilder.RenameColumn(
                name: "BarberId",
                table: "BarberSchedules",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BarberSchedules_BarberId",
                table: "BarberSchedules",
                newName: "IX_BarberSchedules_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarberSchedules_AspNetUsers_UserId",
                table: "BarberSchedules",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
