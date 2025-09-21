namespace UMCore.Templates;

public class CardBaseTemplate
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required int Value { get; init; }
    public required int Boost { get; init; }
    public required string Text { get; init; }
    public required string Script { get; set; } // TODO change to { get; init;}
}