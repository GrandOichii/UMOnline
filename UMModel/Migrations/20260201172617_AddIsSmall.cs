using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMModel.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSmall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSmall",
                table: "Fighters",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSmall",
                table: "Fighters");
        }
    }
}
