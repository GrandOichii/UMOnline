using UMCore.Matches.Players.Cards;

namespace UMCore.Matches.Players.Cards;

public class DiscardPile : MatchCardCollection
{
    public DiscardPile(Player owner) : base(owner)
    {
        ContentsVisibleTo.AddRange(owner.Match.Players.Select(p => p.Idx));
    }
}