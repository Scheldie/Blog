using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class FixCommentDataIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Like_Comments_CommentId",
                table: "Comment_Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Like_Likes_LikeId",
                table: "Comment_Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Like_Posts_PostId",
                table: "Comment_Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment_Like",
                table: "Comment_Like");

            migrationBuilder.RenameTable(
                name: "Comment_Like",
                newName: "Comment_Likes");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Like_PostId",
                table: "Comment_Likes",
                newName: "IX_Comment_Likes_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Like_LikeId",
                table: "Comment_Likes",
                newName: "IX_Comment_Likes_LikeId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Like_CommentId",
                table: "Comment_Likes",
                newName: "IX_Comment_Likes_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment_Likes",
                table: "Comment_Likes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Likes_Comments_CommentId",
                table: "Comment_Likes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Likes_Likes_LikeId",
                table: "Comment_Likes",
                column: "LikeId",
                principalTable: "Likes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Likes_Posts_PostId",
                table: "Comment_Likes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.Sql(@"
            UPDATE comments 
            SET updated_at = created_at 
            WHERE updated_at = '-infinity' OR updated_at IS NULL;
            
            ALTER TABLE comments 
            ALTER COLUMN likes_count SET DEFAULT 0;
            
            UPDATE comments 
            SET likes_count = 0 
            WHERE likes_count IS NULL;
        ");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Likes_Comments_CommentId",
                table: "Comment_Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Likes_Likes_LikeId",
                table: "Comment_Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Likes_Posts_PostId",
                table: "Comment_Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment_Likes",
                table: "Comment_Likes");

            migrationBuilder.RenameTable(
                name: "Comment_Likes",
                newName: "Comment_Like");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Likes_PostId",
                table: "Comment_Like",
                newName: "IX_Comment_Like_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Likes_LikeId",
                table: "Comment_Like",
                newName: "IX_Comment_Like_LikeId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Likes_CommentId",
                table: "Comment_Like",
                newName: "IX_Comment_Like_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment_Like",
                table: "Comment_Like",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Like_Comments_CommentId",
                table: "Comment_Like",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Like_Likes_LikeId",
                table: "Comment_Like",
                column: "LikeId",
                principalTable: "Likes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Like_Posts_PostId",
                table: "Comment_Like",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments"
            );
            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments",
                column: "ParentId",
                principalTable: "Comments",
                principalColumn: "Id"
            );
        }
    }
}
