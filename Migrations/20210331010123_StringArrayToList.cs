using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace leashApi.Migrations
{
    public partial class StringArrayToList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenSub",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "canEdit",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "canRead",
                table: "Pictures");

            migrationBuilder.AddColumn<int>(
                name: "TokenSubId",
                table: "UserData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PictureId",
                table: "TokenSubs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PictureId1",
                table: "TokenSubs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserData_TokenSubId",
                table: "UserData",
                column: "TokenSubId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenSubs_PictureId",
                table: "TokenSubs",
                column: "PictureId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenSubs_PictureId1",
                table: "TokenSubs",
                column: "PictureId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenSubs_Pictures_PictureId",
                table: "TokenSubs",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TokenSubs_Pictures_PictureId1",
                table: "TokenSubs",
                column: "PictureId1",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_TokenSubs_TokenSubId",
                table: "UserData",
                column: "TokenSubId",
                principalTable: "TokenSubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenSubs_Pictures_PictureId",
                table: "TokenSubs");

            migrationBuilder.DropForeignKey(
                name: "FK_TokenSubs_Pictures_PictureId1",
                table: "TokenSubs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserData_TokenSubs_TokenSubId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_TokenSubId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_TokenSubs_PictureId",
                table: "TokenSubs");

            migrationBuilder.DropIndex(
                name: "IX_TokenSubs_PictureId1",
                table: "TokenSubs");

            migrationBuilder.DropColumn(
                name: "TokenSubId",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "TokenSubs");

            migrationBuilder.DropColumn(
                name: "PictureId1",
                table: "TokenSubs");

            migrationBuilder.AddColumn<string>(
                name: "TokenSub",
                table: "UserData",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "canEdit",
                table: "Pictures",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "canRead",
                table: "Pictures",
                type: "text[]",
                nullable: true);
        }
    }
}
