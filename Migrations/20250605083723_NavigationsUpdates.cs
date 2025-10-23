using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class NavigationsUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AlterColumn<int>(
                name: "LikeId",
                table: "Post_Likes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
    
        }
    }
}
