namespace UMDto;

public class FighterDto
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
    public required string Script { get; init; }    
    public required bool CanMoveOverOpposing { get; init; }
    public required int MeleeRange { get; init; }
}