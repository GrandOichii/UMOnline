using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches.Cards;

public class MatchCard
{
    public Player Owner { get; }
    public Card Card { get; }
    public int Idx { get; }

    public string LogName => $"{{{Idx}}}{Card.Template.Name}[{Owner.Idx}]";

    public MatchCard(Player owner, Card card)
    {
        Owner = owner;
        Card = card;

        Idx = owner.Match.AddCard(this);
    }

    public int GetBoostValue()
    {
        return Card.Template.Boost;
    }    

    public bool CanBePlayedAsScheme(Fighter fighter)
    {
        return Card.Template.Type == "Scheme" && Card.CanBePlayedBy(fighter.Name);
    }

    public IEnumerable<Fighter> GetCanBePlayedBy()
    {
        return Owner.Fighters.Where(f => CanBePlayedAsScheme(f));
    }
}