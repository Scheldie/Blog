using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.OldMigrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: DateTime.UtcNow),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: DateTime.UtcNow),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvatarPath = table.Column<string>(type: null, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
            
            // Создаём триггер
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION update_User_timestamp()
            RETURNS TRIGGER AS $$
            BEGIN
                NEW.""LastUpdatedAt"" = NOW();
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;

            CREATE TRIGGER trigger_update_User_timestamp
            BEFORE UPDATE ON ""Users""
            FOR EACH ROW
            EXECUTE FUNCTION update_User_timestamp();
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trigger_update_User_timestamp ON Users");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_User_timestamp()");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
