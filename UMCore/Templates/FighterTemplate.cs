namespace UMCore.Templates;

public class FighterTemplate
{
    public required string Name { get; set; }
    public required string Key { get; set; }
    public required int Amount { get; set; }
    public required string Text { get; set; }
    public required int MaxHealth { get; set; }
    public required int StartingHealth { get; set; }
    public required bool IsHero { get; set; }
    public required int Movement { get; set; }
    public required bool IsRanged { get; set; }
    public required string Script { get; set; }    
    public int MeleeRange { get; set; } = 1;
}