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

public class CardZoneChange(MatchCard card, ICardZone fromZone, ICardZone targetZone, ZoneChangeLocation location, ZoneChangeType type) {
    public MatchCard Card { get; } = card;
    public ICardZone FromZone { get; } = fromZone;
    public ICardZone TargetZone { get; set; } = targetZone;
    public ZoneChangeLocation Location { get; set; } = location;
    public ZoneChangeType Type { get; } = type;

    public void Resolve()
    {
        var redirectors = Card.Owner.Match.GetCardZoneChangeRedirectors();
        foreach (var redirector in redirectors)
        {
            var redirected = redirector.Redirect(this);
            if (redirected) break;
        }

        FromZone.Remove(Card);
        TargetZone.Add(Card, Location);
    }

}