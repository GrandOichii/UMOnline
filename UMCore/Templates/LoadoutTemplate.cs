namespace UMCore.Templates;

public class LoadoutTemplate
{
    public required string Name { get; set; }
    public required bool StartsWithSidekicks { get; set; }
    public required List<FighterTemplate> Fighters { get; init; }
    public required List<CardTemplate> Deck { get; init; }
    public required bool ChoosesSidekick { get; init; }
    public required List<string> StartsWithCards { get; init; }
    public required List<string> CantBePlayedWith { get; init; }
    public required int? StartingHandSize { get; set; }
    public required int? MaximumHandSize { get; set; }
}