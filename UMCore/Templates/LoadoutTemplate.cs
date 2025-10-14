namespace UMCore.Templates;

public class LoadoutTemplate
{
    public required string Name { get; set; }
    public required bool StartsWithSidekicks { get; set; }
    public required List<FighterTemplate> Fighters { get; init; }
    public required List<LoadoutCardTemplate> Deck { get; init; }
}

public class LoadoutCardTemplate
{
    public required int Amount { get; init; }   
    public required CardTemplate Card { get; init; }
}