namespace UMCore.Templates;

public class CardTemplate
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required int Value { get; init; }
    public required int Boost { get; init; }
    public required string Text { get; init; }
    public required string Script { get; set; } // TODO change to { get; init;}
    public required List<string> AllowedFighters { get; init; }

    public bool CanBePlayedBy(string name)
    {
        return AllowedFighters.Count == 0 || AllowedFighters.Contains(name);
    }
}