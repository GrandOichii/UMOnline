using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UMModel.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatchConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Configs",
                keyColumn: "Name",
                keyValue: "Seed 0 tester");

            migrationBuilder.InsertData(
                table: "Configs",
                columns: new[] { "Name", "ActionsPerTurn", "ExhaustDamage", "FirstPlayerIdx", "InitialHandSize", "ManoeuvreDrawAmount", "MaxHandSize", "RandomFirstPlayer", "RandomMatch", "Seed", "TeamCount", "TeamSize" },
                values: new object[,]
                {
                    { "2 vs 2", 2, 2, -1, 5, 1, 7, true, true, 0, 2, 2 },
                    { "4 free for all", 2, 2, -1, 5, 1, 7, true, true, 0, 4, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Configs",
                keyColumn: "Name",
                keyValue: "2 vs 2");

            migrationBuilder.DeleteData(
                table: "Configs",
                keyColumn: "Name",
                keyValue: "4 free for all");

            migrationBuilder.InsertData(
                table: "Configs",
                columns: new[] { "Name", "ActionsPerTurn", "ExhaustDamage", "FirstPlayerIdx", "InitialHandSize", "ManoeuvreDrawAmount", "MaxHandSize", "RandomFirstPlayer", "RandomMatch", "Seed", "TeamCount", "TeamSize" },
                values: new object[] { "Seed 0 tester", 2, 2, -1, 5, 1, 7, true, false, 0, 2, 1 });
        }
    }
}
