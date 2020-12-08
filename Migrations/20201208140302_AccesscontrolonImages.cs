using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace leashApi.Migrations
{
    public partial class AccesscontrolonImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Pictures",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string[]>(
                name: "canEdit",
                table: "Pictures",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "canRead",
                table: "Pictures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "canEdit",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "canRead",
                table: "Pictures");
        }
    }
}
