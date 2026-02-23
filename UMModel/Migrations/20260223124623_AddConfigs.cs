using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UMModel.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RandomMatch = table.Column<bool>(type: "boolean", nullable: false),
                    Seed = table.Column<int>(type: "integer", nullable: false),
                    InitialHandSize = table.Column<int>(type: "integer", nullable: false),
                    ActionsPerTurn = table.Column<int>(type: "integer", nullable: false),
                    MaxHandSize = table.Column<int>(type: "integer", nullable: false),
                    ManoeuvreDrawAmount = table.Column<int>(type: "integer", nullable: false),
                    RandomFirstPlayer = table.Column<bool>(type: "boolean", nullable: false),
                    FirstPlayerIdx = table.Column<int>(type: "integer", nullable: false),
                    ExhaustDamage = table.Column<int>(type: "integer", nullable: false),
                    TeamSize = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Configs",
                columns: new[] { "Id", "ActionsPerTurn", "ExhaustDamage", "FirstPlayerIdx", "InitialHandSize", "ManoeuvreDrawAmount", "MaxHandSize", "RandomFirstPlayer", "RandomMatch", "Seed", "TeamSize" },
                values: new object[] { 0, 2, 2, -1, 5, 1, 7, true, true, 0, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");
        }
    }
}
