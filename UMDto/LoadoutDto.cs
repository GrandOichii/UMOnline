namespace UMDto;

public class LoadoutDto
{
    public required string Name { get; init; }
    public required bool StartsWithSidekicks { get; init; }
    public required List<FighterDto> Fighters { get; init; }
    public required List<CardDto> Deck { get; init; }
    public required bool ChoosesSidekick { get; init; }
    public required List<string> StartsWithCards { get; init; }
    public required List<string> CantBePlayedWith { get; init; }
    public required int? StartingHandSize { get; init; }
    public required int? MaximumHandSize { get; init; }
}
