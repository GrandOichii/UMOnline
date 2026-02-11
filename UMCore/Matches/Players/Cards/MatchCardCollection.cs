using Microsoft.Extensions.Logging;
using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public abstract class MatchCardCollection : IHasData<MatchCardCollection.Data>, ICardZone
{
    public Player Owner { get; }
    public List<MatchCard> Cards { get; private set; }
    public List<int> ContentsVisibleTo { get; }
    private readonly string _name;

    public string ZoneLogName => ICardZone.GetZoneLogName(GetName(), GetOwner());

    public MatchCardCollection(Player owner, string name)
    {
        _name = name;
        Owner = owner;
        Cards = [];
        ContentsVisibleTo = [];
    }

    public MatchCard GetCardByID(int id) => Cards.First(c => c.Id == id);

    public MatchCard GetFirstCardWithKey(string key) => Cards.First(c => c.Template.Key == key);

    public int Count => Cards.Count;

    public async Task AddRaw(IEnumerable<MatchCard> cards)
    {
        Cards.AddRange(cards);

        await Owner.Match.UpdateClients();
    }

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
        Owner.Match.Logger?.LogDebug("Shuffling MatchCardCollection {ZoneLogName}", ZoneLogName);
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
    public Player GetOwner() => Owner;

    public List<MatchCard> GetTopCards(int amount)
    {
        List<MatchCard> result = [];

        amount = Math.Min(amount, Cards.Count);
        for (int i = 0; i < amount; ++i)
        {
            result.Add(Cards[i]);
        }

        return result;
    }

    public async Task<List<MatchCard>> MoveTopCardsTo(int amount, MatchCardCollection targetZone, ZoneChangeType type, ZoneChangeLocation location = ZoneChangeLocation.BOTTOM)
    {
        List<MatchCard> result = GetTopCards(amount);
        foreach (var card in result)
        {
            card.Move(this, targetZone, location, type);
        }

        return result;
    }

    public class Data
    {
        public required MatchCard.Data?[] Cards { get; init; }
        public required int Count { get; init; }
    }
}