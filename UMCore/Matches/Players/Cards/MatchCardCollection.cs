using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public abstract class MatchCardCollection : IHasData<MatchCardCollection.Data>
{
    public Player Owner { get; }
    public List<MatchCard> Cards { get; private set; }
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

    public bool IsPublicFor(Player player) {
        return ContentsVisibleTo.Contains(player.Idx);
    }

    public void Shuffle()
    {
        Cards = [.. Cards.OrderBy(_ => Owner.Match.Random.Next())];
    }

    public virtual Data GetData(Player player)
    {
        return new()
        {
            Count = Cards.Count,
            Cards = IsPublicFor(player)
                ? Cards.Select(c => c.GetData(player)).ToArray()
                : Cards.Select<MatchCard, MatchCard.Data?>(_ => null).ToArray()
                ,
        };
    }

    public class Data
    {
        public required MatchCard.Data?[] Cards { get; init; }
        public required int Count { get; init; }
    }
}