using UMCore.Templates;

namespace UMCore.Matches.Cards;

public class MatchCard
{
    public CardTemplate Template { get; }

    public MatchCard(CardTemplate template)
    {
        Template = template;
    }

    
}