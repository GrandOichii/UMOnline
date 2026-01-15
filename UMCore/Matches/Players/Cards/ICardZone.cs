using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public interface ICardZone {
    void Add(MatchCard card, ZoneChangeLocation location);
    void Remove(MatchCard card);

    Player GetOwner();
    string GetName();

    public string ZoneLogName => GetZoneLogName(GetName(), GetOwner());

    public static string GetZoneLogName(string name, Player owner) => $"z_{name}_{owner.LogName}";
}