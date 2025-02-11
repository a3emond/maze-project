using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazeGameBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AssignMediaIdToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE BlogPosts SET MediaId = 10 WHERE Id = 10;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
