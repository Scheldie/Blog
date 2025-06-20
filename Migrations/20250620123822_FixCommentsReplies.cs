using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class FixCommentsReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyTo",
                table: "Comments",
                nullable: true
            );
            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ReplyTo",
                table: "Comments",
                column: "ReplyTo",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyTo",
                table: "Comments"
            );
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ReplyTo",
                table: "Comments"
            );
        }
    }
}
