using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public class Deck : MatchCardCollection
{
    public Deck(Player owner) : base(owner)
    {
    }

    public IEnumerable<MatchCard> TakeFromTop(int amount)
    {
        while (amount-- > 0 && Count > 0)
        {
            var result = Cards[0];
            yield return result;
            Cards.Remove(result);
        }
        // TODO when drawing out of an empty deck deal 2 damage to all owned fighters
    }
}