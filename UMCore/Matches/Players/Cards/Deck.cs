using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public class Deck : MatchCardCollection
{
    public Deck(Player owner) : base(owner)
    {
    }

    public List<MatchCard> TakeFromTop(int amount)
    {
        var result = new List<MatchCard>();
        while (amount-- > 0 && Count > 0)
        {
            var card = Cards[0];
            result.Add(card);
            Cards.Remove(card);
        }
        while (amount-- > 0)
        {
            // TODO when drawing out of an empty deck deal 2 (1?) damage to all owned fighters
        }
        return result;
    }

    public override Data GetData(Player player)
    {
        return new()
        {
            Cards = [],
            Count = Cards.Count,
        };
    }
}