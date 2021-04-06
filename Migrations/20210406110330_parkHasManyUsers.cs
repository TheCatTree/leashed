using Microsoft.EntityFrameworkCore.Migrations;

namespace leashApi.Migrations
{
    public partial class parkHasManyUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "ParkId",
                table: "UserData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserData_ParkId",
                table: "UserData",
                column: "ParkId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_ParkItems_ParkId",
                table: "UserData",
                column: "ParkId",
                principalTable: "ParkItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserData_ParkItems_ParkId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_ParkId",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "ParkId",
                table: "UserData");

            migrationBuilder.AddColumn<long>(
                name: "ParkItemId",
                table: "UserData",
                type: "bigint",
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
    }
}
