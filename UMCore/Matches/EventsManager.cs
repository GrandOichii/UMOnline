using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore.Matches;

public class EventsManager(Match match)
{
    public void SchemePlayed(MatchCard card, Fighter fighter)
    {
        foreach (var p in match.Players)
        {
            p.AddEvent(new SchemeEvent()
            {
                Card = card.GetData(p),
                Fighter = fighter.GetData(p),
                PlayerIdx = card.Owner.Idx,
                PlayerName = card.Owner.Name,
            });
        }
    }
}

public class Event(string eType)
{
    public string EventType { get; } = eType;
}

public class SchemeEvent : Event
{
    public SchemeEvent() : base("scheme")
    {
    }

    public required int PlayerIdx { get; init; }
    public required string PlayerName { get; init; }
    public required MatchCard.Data Card { get; init; }
    public required Fighter.Data Fighter { get; init; }
}