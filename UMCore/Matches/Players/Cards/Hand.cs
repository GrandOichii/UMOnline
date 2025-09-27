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
        var newCards = await Owner.Deck.TakeFromTop(amount);
        Cards.AddRange(newCards);

        Owner.Match.Logger?.LogDebug("Player {PlayerLogName} draws {Amount} cards (wanted to draw: {OriginalAmount})", Owner.LogName, newCards.Count(), amount);
        Owner.Match.Logs.Private(
            Owner,
            string.Format("You drew cards: {0}", string.Join(", ", newCards.Select(c => c.FormattedLogName))),
            $"Player {Owner.FormattedLogName} drew {newCards.Count} cards"
        );
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

    public async Task Discard(MatchCard card, bool log=true)
    {
        if (!Cards.Contains(card))
        {
            throw new Exception($"Cannot discard card {card.LogName} from hand of {Owner.LogName} for they do not have it"); // TODO type and weird message
        }

        Owner.Match.Logger?.LogDebug("Player {PlayerLogName} discards {CardLogName} from their hand", Owner.LogName, card.LogName);
        if (log)
        {
            Owner.Match.Logs.Public($"Player {Owner.FormattedLogName} discards {card.FormattedLogName} from their hand");
        }

        Cards.Remove(card);
        await Owner.DiscardPile.Add([card]);
    }
}