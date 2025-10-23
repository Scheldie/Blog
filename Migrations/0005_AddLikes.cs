using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.OldMigrations
{
    /// <inheritdoc />
    public partial class AddLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'LikeType') THEN
                        CREATE TYPE LikeType AS ENUM ('Post', 'Comment');
                    END IF;
                END$$;"
            );

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    LikeType = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade //При удалении пользователя, удаляются его лайки
                    );

                }
            );
            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_EntityId_LikeType",
                table: "Likes",
                columns: new[] { "UserId", "EntityId", "LikeType" },
                unique: true
            );
            migrationBuilder.CreateIndex(
               name: "IX_Likes_EntityId_LikeType",
               table: "Likes",
               columns: new[] { "EntityId", "LikeType" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes"
            );
            migrationBuilder.Sql("DROP TYPE IF EXISTS LikeType;");
        }
    }
}
