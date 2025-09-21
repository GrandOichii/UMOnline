using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public class Hand : MatchCardCollection
{
    public Hand(Player owner) : base(owner)
    {
        ContentsVisibleTo.Add(owner.Idx);
    }

    public async Task<IEnumerable<MatchCard>> Draw(int amount)
    {
        var newCards = Owner.Deck.TakeFromTop(amount);
        Cards.AddRange(newCards);

        return newCards;
    }

    public override async Task Add(IEnumerable<MatchCard> cards)
    {
        await base.Add(cards);

        // TODO update clients
    }

    public IEnumerable<MatchCard> GetPlayableSchemeCards()
    {
        List<MatchCard> result = [];
        foreach (var fighter in Owner.Fighters)
        {
            result.AddRange(Cards.Where(c => c.CanBePlayedAsScheme(fighter)));
        }
        return result.ToHashSet();
    }
}