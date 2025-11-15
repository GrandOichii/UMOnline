using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public enum ZoneChangeLocation
{
    TOP,
    BOTTOM
}

public class CardZoneChange(MatchCard card, ICardZone fromZone, ICardZone targetZone, ZoneChangeLocation location) {
    public MatchCard Card { get; } = card;
    public ICardZone FromZone { get; } = fromZone;
    public ICardZone TargetZone { get; set; } = targetZone;
    public ZoneChangeLocation Location { get; } = location;

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