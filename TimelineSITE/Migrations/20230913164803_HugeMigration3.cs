using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimelineSITE.Migrations
{
    public partial class HugeMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumsAcceses_Albums_AlbumId",
                table: "AlbumsAcceses");

            migrationBuilder.DropForeignKey(
                name: "FK_AlbumsAcceses_AspNetUsers_UserId",
                table: "AlbumsAcceses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumsAcceses",
                table: "AlbumsAcceses");

            migrationBuilder.RenameTable(
                name: "AlbumsAcceses",
                newName: "AlbumsAccesses");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumsAcceses_UserId",
                table: "AlbumsAccesses",
                newName: "IX_AlbumsAccesses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumsAcceses_AlbumId",
                table: "AlbumsAccesses",
                newName: "IX_AlbumsAccesses_AlbumId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumsAccesses",
                table: "AlbumsAccesses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumsAccesses_Albums_AlbumId",
                table: "AlbumsAccesses",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumsAccesses_AspNetUsers_UserId",
                table: "AlbumsAccesses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumsAccesses_Albums_AlbumId",
                table: "AlbumsAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_AlbumsAccesses_AspNetUsers_UserId",
                table: "AlbumsAccesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumsAccesses",
                table: "AlbumsAccesses");

            migrationBuilder.RenameTable(
                name: "AlbumsAccesses",
                newName: "AlbumsAcceses");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumsAccesses_UserId",
                table: "AlbumsAcceses",
                newName: "IX_AlbumsAcceses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumsAccesses_AlbumId",
                table: "AlbumsAcceses",
                newName: "IX_AlbumsAcceses_AlbumId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumsAcceses",
                table: "AlbumsAcceses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumsAcceses_Albums_AlbumId",
                table: "AlbumsAcceses",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumsAcceses_AspNetUsers_UserId",
                table: "AlbumsAcceses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
