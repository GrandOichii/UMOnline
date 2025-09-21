namespace UMCore.Templates;

public class CardTemplate
{
    public required CardBaseTemplate Template { get; init; }

    public required List<string> AllowedFighters { get; init; }

    public bool CanBePlayedBy(string name)
    {
        return AllowedFighters.Count == 0 || AllowedFighters.Contains(name);
    }
}