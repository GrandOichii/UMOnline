using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMModel.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loadouts",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartsWithSidekicks = table.Column<bool>(type: "boolean", nullable: false),
                    ChoosesSidekick = table.Column<bool>(type: "boolean", nullable: false),
                    StartsWithCards = table.Column<List<string>>(type: "text[]", nullable: false),
                    CantBePlayedWith = table.Column<List<string>>(type: "text[]", nullable: false),
                    StartingHandSize = table.Column<int>(type: "integer", nullable: true),
                    MaximumHandSize = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loadouts", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: true),
                    Boost = table.Column<int>(type: "integer", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Script = table.Column<string>(type: "text", nullable: false),
                    AllowedFighters = table.Column<List<string>>(type: "text[]", nullable: false),
                    Labels = table.Column<string[]>(type: "text[]", nullable: false),
                    IncludedInDeckWithSidekick = table.Column<string>(type: "text", nullable: true),
                    LoadoutName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Cards_Loadouts_LoadoutName",
                        column: x => x.LoadoutName,
                        principalTable: "Loadouts",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fighters",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    MaxHealth = table.Column<int>(type: "integer", nullable: false),
                    StartingHealth = table.Column<int>(type: "integer", nullable: false),
                    IsHero = table.Column<bool>(type: "boolean", nullable: false),
                    Movement = table.Column<int>(type: "integer", nullable: false),
                    IsRanged = table.Column<bool>(type: "boolean", nullable: false),
                    Script = table.Column<string>(type: "text", nullable: false),
                    CanMoveOverOpposing = table.Column<bool>(type: "boolean", nullable: false),
                    MeleeRange = table.Column<int>(type: "integer", nullable: false),
                    LoadoutName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fighters", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Fighters_Loadouts_LoadoutName",
                        column: x => x.LoadoutName,
                        principalTable: "Loadouts",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_LoadoutName",
                table: "Cards",
                column: "LoadoutName");

            migrationBuilder.CreateIndex(
                name: "IX_Fighters_LoadoutName",
                table: "Fighters",
                column: "LoadoutName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Fighters");

            migrationBuilder.DropTable(
                name: "Loadouts");
        }
    }
}
