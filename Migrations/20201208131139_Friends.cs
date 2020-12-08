using Microsoft.EntityFrameworkCore.Migrations;

namespace leashApi.Migrations
{
    public partial class Friends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserDataId",
                table: "UserData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserData_UserDataId",
                table: "UserData",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_UserData_UserDataId",
                table: "UserData",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserData_UserData_UserDataId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_UserDataId",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "UserData");
        }
    }
}
