using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public class Hand : MatchCardCollection
{
    public Hand(Player owner) : base(owner, "HAND")
    {
        ContentsVisibleTo.Add(owner.Idx);
    }

    public async Task<IEnumerable<MatchCard>> Draw(int amount)
    {
        var newCards = await Owner.Deck.MoveTopCardsTo(amount, this, ZoneChangeLocation.BOTTOM);

        Owner.Match.Logger?.LogDebug("Player {PlayerLogName} draws {Amount} cards (wanted to draw: {OriginalAmount})", Owner.LogName, newCards.Count(), amount);
        Owner.Match.Logs.Private(
            Owner,
            newCards.Count > 0
                ? string.Format("You drew cards: {0}", string.Join(", ", newCards.Select(c => c.FormattedLogName)))
                : "You drew no cards", 
            $"Player {Owner.FormattedLogName} drew {newCards.Count} cards"
        );

        var exhaustTimes = amount - newCards.Count;
        if (exhaustTimes > 0)
        {
            await Owner.Exhaust(amount);
        }
        
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
        card.Move(this, Owner.DiscardPile, ZoneChangeLocation.TOP);
        await Owner.Match.UpdateClients();

        Owner.Match.Logger?.LogDebug("Player {PlayerLogName} discards {CardLogName} from their hand", Owner.LogName, card.LogName);
        if (log)
        {
            Owner.Match.Logs.Public($"Player {Owner.FormattedLogName} discards {card.FormattedLogName} from their hand");
        }
    }
}