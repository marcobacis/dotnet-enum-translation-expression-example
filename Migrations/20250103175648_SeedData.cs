using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnumTranslator.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("MyEntities", new[] { "Name", "Type" }, new[] { "Default", "0" });
            migrationBuilder.InsertData("MyEntities", new[] { "Name", "Type" }, new[] { "Example", "1" });
            migrationBuilder.InsertData("MyEntities", new[] { "Name", "Type" }, new[] { "OtherValue", "2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MyEntities");
        }
    }
}
