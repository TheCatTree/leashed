using Microsoft.EntityFrameworkCore.Migrations;

namespace leashApi.Migrations
{
    public partial class AddingParkGoers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParkItemId",
                table: "UserData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserData_ParkItemId",
                table: "UserData",
                column: "ParkItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_ParkItems_ParkItemId",
                table: "UserData",
                column: "ParkItemId",
                principalTable: "ParkItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserData_ParkItems_ParkItemId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_ParkItemId",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "ParkItemId",
                table: "UserData");
        }
    }
}
