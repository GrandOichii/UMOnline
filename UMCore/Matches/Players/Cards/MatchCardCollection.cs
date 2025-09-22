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

    public async Task Add(IEnumerable<MatchCard> cards)
    {
        Cards.AddRange(cards);

        // TODO update clients
    }

    public async Task<bool> Remove(MatchCard card)
    {
        var result = Cards.Remove(card);

        // TODO update clients
        
        return result;
    }

    public int GetCardIdx(MatchCard card)
    {
        var result = Cards.FindIndex(c => c == card);
        if (result < 0)
        {
            // TODO throw
        }
        return result;
    }

}