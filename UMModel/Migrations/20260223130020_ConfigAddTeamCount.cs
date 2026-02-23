using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMModel.Migrations
{
    /// <inheritdoc />
    public partial class ConfigAddTeamCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamCount",
                table: "Configs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Configs",
                keyColumn: "Name",
                keyValue: "1 vs 1",
                column: "TeamCount",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Configs",
                keyColumn: "Name",
                keyValue: "Seed 0 tester",
                column: "TeamCount",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamCount",
                table: "Configs");
        }
    }
}
