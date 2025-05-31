using Blog.Entites;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Blog.Migrations
{
    public class AddNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    NotificationType = table.Column<string>(nullable: false),
                    RelatedId = table.Column<int>(nullable: false),
                    RelatedType = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "CreatedAt" }
            );
            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedId_RelatedType",
                table: "Notifications",
                columns: new[] { "RelatedId", "RelatedType" }
            );
            migrationBuilder.Sql(
                @"ALTER TABLE Notifications 
                  ADD CONSTRAINT check_NotificationType 
                  CHECK (NotificationType IN ('post', 'comment', 'like', 'follow'))"
            );
            migrationBuilder.Sql(
                @"ALTER TABLE Notifications 
                  ADD CONSTRAINT check_RelatedType 
                  CHECK (RelatedType IN ('post', 'comment', 'user'))"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
