using Microsoft.EntityFrameworkCore.Migrations;

namespace leashApi.Migrations
{
    public partial class UserImagelink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_UserData_UserDataId",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "Pictures");

            migrationBuilder.AlterColumn<int>(
                name: "UserDataId",
                table: "Pictures",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Pictures",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_UserData_UserDataId",
                table: "Pictures",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_UserData_UserDataId",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Pictures");

            migrationBuilder.AlterColumn<int>(
                name: "UserDataId",
                table: "Pictures",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "Pictures",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "Pictures",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_UserData_UserDataId",
                table: "Pictures",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
