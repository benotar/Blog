using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostTableIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_Category_Slug_Title",
                table: "Posts",
                columns: new[] { "Category", "Slug", "Title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Category_Slug_Title",
                table: "Posts");
        }
    }
}
