using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Blog.Migrations
{
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("PostgreSQL:Autoincrement", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false, maxLength: 1200),
                    PostId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false, type: "datetime2"),
                    UpdatedAt = table.Column<DateTime>(nullable: false, type: "datetime2")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade // Удаляет все комментарии пользователя при удалении его профиля
                    );
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade // Удаляет все комментарии из поста при удалении поста
                    );
                    //table.ForeignKey(
                    //    name: "FK_Comments_Comments_ParentId",
                    //    column: x => x.ParentId,
                    //    principalTable: "Comments",
                    //    principalColumn: "Id",
                    //    onDelete: ReferentialAction.Cascade // Удаляет все ответы на родительский комментарий
                    //);
                }
            );
            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId"
            );
            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId"
            );
            
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments"
            );
        }
    }
}
