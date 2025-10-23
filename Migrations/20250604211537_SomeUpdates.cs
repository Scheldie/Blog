using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class SomeUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.CreateIndex(
                name: "IX_Post_Likes_PostId",
                table: "Post_Likes",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Likes_PostId"
                );



        }
    }
}
