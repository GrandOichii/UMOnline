using System.Threading.Tasks;
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

    public IEnumerable<MatchCard> GetPlayableSchemeCards()
    {
        List<MatchCard> result = [];
        foreach (var fighter in Owner.Fighters)
        {
            result.AddRange(Cards.Where(c => c.CanBePlayedAsScheme(fighter)));
        }
        return result.ToHashSet();
    }

    public async Task Discard(MatchCard card)
    {
        if (!Cards.Contains(card))
        {
            throw new Exception($"Cannot discard card {card.LogName} from hand of {Owner.LogName} for they do not have it"); // TODO type and weird message
        }

        Cards.Remove(card);
        await Owner.DiscardPile.Add([card]);
    }
}