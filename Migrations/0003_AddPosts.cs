using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(nullable: true, maxLength: 100),
                    Description = table.Column<string>(nullable: true, maxLength: 1200),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImagesCount = table.Column<int>(nullable: false),
                    LikesCount = table.Column<int>(nullable: true, defaultValue: 0),
                    ViewCount = table.Column<int>(nullable: true, defaultValue: 0),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade //При удалении пользователя, удаляются его посты
                    );
                }
            );
            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId"
            );
            //Соединительная таблица
            migrationBuilder.CreateTable(
                name: "Post_Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    ImageId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Images_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );                                          // При удалении поста, удаляются
                    table.ForeignKey(                           // связанные записи из соединительной таблицы
                        name: "FK_Post_Images_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                });
            migrationBuilder.CreateIndex(
                name: "IX_Post_Images_PostId_ImageId",
                table: "Post_Images",
                columns: new[] { "PostId", "ImageId" },
                unique: true
            );
            migrationBuilder.CreateIndex(
                name: "IX_Post_Images_PostId_Order",
                table: "Post_Images",
                columns: new[] { "PostId", "Order" }
            );
            migrationBuilder.CreateIndex(
                name: "IX_Post_Images_ImageId",  // Индекс для поиска фото по PostId
                table: "Post_Images",
                column: "ImageId"
            );
            // Соединительная таблица. Нужна для устранения проблем с производительностью
            migrationBuilder.CreateTable(
                name: "Post_Views",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_views", x => new { x.PostId, x.Date });
                    table.ForeignKey(
                        name: "FK_Post_Views_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Индекс для агрегации данных
            migrationBuilder.CreateIndex(
                name: "IX_Post_Views_Date",
                table: "Post_Views",
                column: "Date");

            // Функция для инкремента просмотров
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION increment_Post_View(PostId integer)
                RETURNS void AS $$
                BEGIN
                    INSERT INTO Post_Views (PostId, Date, Count)
                    VALUES (PostId, CURRENT_DATE, 1)
                    ON CONFLICT (PostId, Date)
                    DO UPDATE SET Count = Post_Views.Count + 1;
                END;
                $$ LANGUAGE plpgsql;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts"
            );
            migrationBuilder.DropTable(
                name: "Post_Images"
            );
            migrationBuilder.DropTable(
                name: "Post_Views"
            );
        }
    }
}
