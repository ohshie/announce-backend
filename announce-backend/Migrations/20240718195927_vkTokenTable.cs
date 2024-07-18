using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace announce_backend.Migrations
{
    /// <inheritdoc />
    public partial class vkTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VkTokens",
                columns: table => new
                {
                    ApiToken = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkTokens", x => x.ApiToken);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VkTokens");
        }
    }
}
