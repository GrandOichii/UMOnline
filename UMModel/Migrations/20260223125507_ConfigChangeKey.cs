using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UMModel.Migrations
{
    /// <inheritdoc />
    public partial class ConfigChangeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Configs",
                table: "Configs");

            migrationBuilder.DeleteData(
                table: "Configs",
                keyColumn: "Id",
                keyColumnType: "integer",
                keyValue: 0);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Configs");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Configs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Configs",
                table: "Configs",
                column: "Name");

            migrationBuilder.InsertData(
                table: "Configs",
                columns: new[] { "Name", "ActionsPerTurn", "ExhaustDamage", "FirstPlayerIdx", "InitialHandSize", "ManoeuvreDrawAmount", "MaxHandSize", "RandomFirstPlayer", "RandomMatch", "Seed", "TeamSize" },
                values: new object[,]
                {
                    { "1 vs 1", 2, 2, -1, 5, 1, 7, true, true, 0, 1 },
                    { "Seed 0 tester", 2, 2, -1, 5, 1, 7, true, false, 0, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Configs",
                table: "Configs");

            migrationBuilder.DeleteData(
                table: "Configs",
                keyColumn: "Name",
                keyColumnType: "text",
                keyValue: "1 vs 1");

            migrationBuilder.DeleteData(
                table: "Configs",
                keyColumn: "Name",
                keyColumnType: "text",
                keyValue: "Seed 0 tester");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Configs");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Configs",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Configs",
                table: "Configs",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Configs",
                columns: new[] { "Id", "ActionsPerTurn", "ExhaustDamage", "FirstPlayerIdx", "InitialHandSize", "ManoeuvreDrawAmount", "MaxHandSize", "RandomFirstPlayer", "RandomMatch", "Seed", "TeamSize" },
                values: new object[] { 0, 2, 2, -1, 5, 1, 7, true, true, 0, 1 });
        }
    }
}
