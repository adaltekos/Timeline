using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimelineSITE.Migrations
{
    public partial class DeleteOwnerFromAlbumModelAndAddDateStartDateEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_AspNetUsers_OwnerId",
                table: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_Albums_OwnerId",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Albums");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "Albums",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "Albums",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "Albums");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Albums",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_OwnerId",
                table: "Albums",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_AspNetUsers_OwnerId",
                table: "Albums",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
