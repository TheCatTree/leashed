using Microsoft.EntityFrameworkCore.Migrations;

namespace leashApi.Migrations
{
    public partial class LinkingDogUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "Pictures",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Dogs_UserDataId",
                table: "Dogs",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dogs_UserData_UserDataId",
                table: "Dogs",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dogs_UserData_UserDataId",
                table: "Dogs");

            migrationBuilder.DropIndex(
                name: "IX_Dogs_UserDataId",
                table: "Dogs");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Pictures");
        }
    }
}
