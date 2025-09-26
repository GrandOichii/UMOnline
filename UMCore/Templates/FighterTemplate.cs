namespace UMCore.Templates;

public class FighterTemplate
{
    public required string Name { get; init; }
    public required string Key { get; init; }
    public required int Amount { get; init; }
    public required string Text { get; init; }
    public required int MaxHealth { get; init; }
    public required int StartingHealth { get; init; }
    public required bool IsHero { get; init; }
    public required int Movement { get; init; }
    public required bool IsRanged { get; init; }
    public required string Script { get; set; } // TODO change to { get; init; }
    
    public int MeleeRange { get; init; } = 1;
}