using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazeGameBlazor.Migrations
{
    /// <inheritdoc />
    public partial class RestoredFKinBlogPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_BlogPosts_BlogPostId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_BlogPosts_BlogPostId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_BlogPostId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "BlogPostId",
                table: "Media");

            migrationBuilder.AddColumn<int>(
                name: "MediaId",
                table: "BlogPosts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_MediaId",
                table: "BlogPosts",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Media_MediaId",
                table: "BlogPosts",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_BlogPosts_BlogPostId",
                table: "Likes",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Media_MediaId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_BlogPosts_BlogPostId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_MediaId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "BlogPosts");

            migrationBuilder.AddColumn<int>(
                name: "BlogPostId",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_BlogPostId",
                table: "Media",
                column: "BlogPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_BlogPosts_BlogPostId",
                table: "Likes",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_BlogPosts_BlogPostId",
                table: "Media",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id");
        }
    }
}
