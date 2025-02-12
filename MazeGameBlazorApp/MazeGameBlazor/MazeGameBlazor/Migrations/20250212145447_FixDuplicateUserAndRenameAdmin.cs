using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazeGameBlazor.Migrations
{
    /// <inheritdoc />
    public partial class FixDuplicateUserAndRenameAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️Remove the duplicate "ALEXEMOND" user
            migrationBuilder.Sql(
                "DELETE FROM AspNetUsers WHERE NormalizedUserName = 'ALEXEMOND';"
            );

            // 2️Update "ADMIN" username to "AlexEmond"
            migrationBuilder.Sql(
                "UPDATE AspNetUsers " +
                "SET UserName = 'AlexEmond', NormalizedUserName = 'ALEXEMOND' " +
                "WHERE NormalizedUserName = 'ADMIN';"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
