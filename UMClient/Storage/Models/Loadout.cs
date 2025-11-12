namespace UMClient.Storage.Models;

public class Loadout
{
    public required string Name { get; set; }
    public required bool StartsWithSidekicks { get; set; }
    // public required List<Fighter> Fighters { get; init; }
    // public required List<Card> Deck { get; init; }
    public required bool ChoosesSidekick { get; init; }
    // public required List<string> StartsWithCards { get; init; }
    // public required List<string> CantBePlayedWith { get; init; }d
    public required int? StartingHandSize { get; set; }
    public required int? MaximumHandSize { get; set; }

    public static string GetCreateCommand()
    {
        return """
        CREATE TABLE loadouts(
            name STRING NOT NULL,
            startsWithSidekick INTEGER NOT NULL,
            choosesSidekick INTEGER NOT NULL,
            startingHandSize INTEGER,
            maximumHandSize INTEGER,
        );
        """;
    }
}