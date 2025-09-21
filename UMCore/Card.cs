using UMCore.Templates;

namespace UMCore;

public class Card
{
    public CardTemplate Template { get; }

    public List<string> AllowedFighters { get; }

    public Card(CardTemplate template)
    {
        Template = template;
        AllowedFighters = [];
    }

    public Card(CardTemplate template, List<string> allowedFighters)
    {
        Template = template;
        AllowedFighters = allowedFighters;
    }

    public bool CanBePlayedBy(string name)
    {
        return AllowedFighters.Count == 0 || AllowedFighters.Contains(name);
    }
}