using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Blog.Migrations
{
    public partial class AddUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    Bio = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable:false, defaultValueSql: "NOW()"),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    LastLogidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageId = table.Column<int>(nullable: true),
    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                }
            );
            // Триггер для обновления updated_at
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_User_timestamp()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.UpdatedAt = NOW();
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER trigger_update_User_timestamp
                BEFORE UPDATE ON Users
                FOR EACH ROW
                EXECUTE FUNCTION update_User_timestamp();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trigger_update_User_timestamp ON Users");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_User_timestamp()");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
