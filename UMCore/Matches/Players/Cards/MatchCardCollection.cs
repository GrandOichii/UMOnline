using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public abstract class MatchCardCollection
{
    public Player Owner { get; }
    public List<MatchCard> Cards { get; }
    public List<int> ContentsVisibleTo { get; }

    public MatchCardCollection(Player owner)
    {
        Owner = owner;
        Cards = [];
        ContentsVisibleTo = [];
    }

    public int Count => Cards.Count;

    public virtual async Task Add(IEnumerable<MatchCard> cards)
    {
        Cards.AddRange(cards);
    }
}