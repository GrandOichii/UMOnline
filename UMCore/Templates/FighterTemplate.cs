namespace UMCore.Templates;

public class FighterTemplate
{
    public required string Name { get; init; }
    public required int MaxHealth { get; init; }
    public required int StartingHealth { get; init; }
    public required bool IsHero { get; init; }
    public required int Movement { get; init; }
}