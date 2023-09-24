using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimelineSITE.Migrations
{
    public partial class UpdateAlbumAccessModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AlbumsAccesses_AlbumId",
                table: "AlbumsAccesses");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumsAccesses_AlbumId_UserId",
                table: "AlbumsAccesses",
                columns: new[] { "AlbumId", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AlbumsAccesses_AlbumId_UserId",
                table: "AlbumsAccesses");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumsAccesses_AlbumId",
                table: "AlbumsAccesses",
                column: "AlbumId");
        }
    }
}
