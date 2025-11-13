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
    public ICardZone TargetZone { get; private set; } = targetZone;
    public ZoneChangeLocation Location { get; } = location;

    public void Resolve()
    {
        FromZone.Remove(Card);
        TargetZone.Add(Card, Location);
    }

}