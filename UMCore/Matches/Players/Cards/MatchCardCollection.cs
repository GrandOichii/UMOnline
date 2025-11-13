using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public abstract class MatchCardCollection : IHasData<MatchCardCollection.Data>, ICardZone
{
    public Player Owner { get; }
    public List<MatchCard> Cards { get; private set; }
    public List<int> ContentsVisibleTo { get; }
    private readonly string _name;

    public MatchCardCollection(Player owner, string name)
    {
        _name = name;
        Owner = owner;
        Cards = [];
        ContentsVisibleTo = [];
    }

    public MatchCard GetFirstCardWithKey(string key) => Cards.First(c => c.Template.Key == key);

    public int Count => Cards.Count;

    // public void MoveCard(MatchCard card, ICardZone from) {

    // }

    public async Task AddRaw(IEnumerable<MatchCard> cards)
    {
        Cards.AddRange(cards);

        await Owner.Match.UpdateClients();
    }

    // public async Task<bool> Remove(MatchCard card)
    // {
    //     var result = Cards.Remove(card);

    //     await Owner.Match.UpdateClients();

    //     return result;
    // }

    public int GetCardIdx(MatchCard card)
    {
        var result = Cards.FindIndex(c => c == card);
        if (result < 0)
        {
            throw new MatchException($"Failed to find CardIdx of card {card.LogName}");
        }
        return result;
    }

    public bool IsPublicFor(Player player) {
        return ContentsVisibleTo.Contains(-1) || ContentsVisibleTo.Contains(player.Idx);
    }

    public void Shuffle()
    {
        Cards = [.. Cards.OrderBy(_ => Owner.Match.Random.Next())];
    }

    // public async Task PutOnBottom(MatchCard card)
    // {
    //     Cards.Add(card);
    //     await Owner.Match.UpdateClients();
    // }

    // public virtual async Task<List<MatchCard>> TakeFromTop(int amount)
    // {
    //     var result = new List<MatchCard>();
    //     while (Count > 0 && amount-- > 0)
    //     {
    //         var card = Cards[0];
    //         result.Add(card);
    //         Cards.Remove(card);
    //     }
    //     return result;
    // }


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

    public void Add(MatchCard card, ZoneChangeLocation location)
    {
        switch (location)
        {
            case ZoneChangeLocation.TOP:
                Cards.Insert(0, card);
                break;
            case ZoneChangeLocation.BOTTOM:
                Cards.Add(card);
                break;
        }
    }

    public void Remove(MatchCard card)
    {
        var removed = Cards.Remove(card);
        if (removed) return; 
        throw new MatchException($"Tried to remove card {card.LogName} from collection {_name} of player {Owner.LogName}, while it was not there");
    }

    public string GetName() => _name;

    public async Task<List<MatchCard>> MoveTopCardsTo(int amount, MatchCardCollection targetZone, ZoneChangeLocation location = ZoneChangeLocation.BOTTOM)
    {
        List<MatchCard> result = [];

        while (amount-- > 0 && Cards.Count > 0)
        {
            var card = Cards[0];
            card.Move(this, targetZone, location);
            result.Add(card);
        }

        return result;
    }

    public class Data
    {
        public required MatchCard.Data?[] Cards { get; init; }
        public required int Count { get; init; }
    }
}