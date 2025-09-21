using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        var newCards = Owner.Deck.TakeFromTop(amount).ToList();
        Cards.AddRange(newCards);

        Owner.Match.Logger?.LogDebug("Player {PlayerLogName} draws {Amount} cards (wanted to draw: {OriginalAmount})", Owner.LogName, newCards.Count(), amount);
        return newCards;
    }

    public IEnumerable<MatchCard> GetPlayableSchemeCards()
    {
        List<MatchCard> result = [];
        foreach (var fighter in Owner.GetAliveFighters())
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