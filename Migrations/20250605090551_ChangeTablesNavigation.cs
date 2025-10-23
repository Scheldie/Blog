using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.OldMigrations
{
    /// <inheritdoc />
    public partial class ChangeTablesNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments",
                column: "ParentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments"
            );
            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentId",
                table: "Comments"
            );
        }
    }
}
