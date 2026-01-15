using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public enum ZoneChangeLocation
{
    TOP = 1,
    BOTTOM = 2,
}

public enum ZoneChangeType
{
    TODO = 0,
    PLAYED = 1,
    DISCARDED = 2,
}

public class CardZoneChange(
    MatchCard card,
    ICardZone fromZone,
    ICardZone targetZone,
    ZoneChangeLocation location,
    ZoneChangeType type
)
{
    public MatchCard Card { get; } = card;
    public ICardZone FromZone { get; } = fromZone;
    public ICardZone TargetZone { get; set; } = targetZone;
    public ZoneChangeLocation Location { get; set; } = location;
    public ZoneChangeType Type { get; } = type;

    public void Resolve()
    {
        Card.Owner.Match.Logger?.LogDebug("Initiating card zone change of card {CardLogName} from zone {FromZoneLogName} to {ToZoneLogName} (location: {ZoneChangeLocation}, type: {ZoneChangeType})", Card.LogName, FromZone.ZoneLogName, TargetZone.ZoneLogName, Location, Type);

        var redirectors = Card.Owner.Match.GetCardZoneChangeRedirectors().ToList();
        foreach (var redirector in redirectors)
        {
            Card.Owner.Match.Logger?.LogDebug("Executing possible card zone change redirector by fighter {FighterLogName}", redirector.Fighter.LogName);
            var redirected = redirector.Redirect(this);
            if (redirected)
            {
                Card.Owner.Match.Logger?.LogDebug("Zone change of card {CardLogName} was redirected, skipping all other redirectors", Card.LogName);
                break;
            }
            Card.Owner.Match.Logger?.LogDebug("Redirector of fighter {FighterLogName} didn't redirect card {CardLogName}", redirector.Fighter.LogName, Card.LogName);
        }

        FromZone.Remove(Card);
        TargetZone.Add(Card, Location);
    }

}