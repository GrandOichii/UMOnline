using UMCore.Matches.Players.Cards;

namespace UMCore.Matches.Players.Cards;

public class DiscardPile : MatchCardCollection
{
    public DiscardPile(Player owner) : base(owner, "DISCARD")
    {
        ContentsVisibleTo.Add(-1);
    }
}