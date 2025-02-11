using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazeGameBlazor.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBlogRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM BlogPosts WHERE Id IN (2,3,4,5);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
