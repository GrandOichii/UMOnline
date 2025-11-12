namespace UMDto;

public class CardDto
{
    public required int Amount { get; init; }   
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required int? Value { get; init; }
    public required int? Boost { get; init; }
    public required string Text { get; init; }
    public required string Script { get; init; }
    public required string[] AllowedFighters { get; init; }
    public required string[] Labels { get; init; }
}