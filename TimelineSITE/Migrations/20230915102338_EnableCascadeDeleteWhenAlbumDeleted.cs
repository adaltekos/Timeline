using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimelineSITE.Migrations
{
    public partial class EnableCascadeDeleteWhenAlbumDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumsAccesses_Albums_AlbumId",
                table: "AlbumsAccesses");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumsAccesses_Albums_AlbumId",
                table: "AlbumsAccesses",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumsAccesses_Albums_AlbumId",
                table: "AlbumsAccesses");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumsAccesses_Albums_AlbumId",
                table: "AlbumsAccesses",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
