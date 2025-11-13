using UMCore.Matches.Cards;

namespace UMCore.Matches.Players.Cards;

public interface ICardZone {
    void Add(MatchCard card, ZoneChangeLocation location);
    void Remove(MatchCard card);

    string GetName();
}