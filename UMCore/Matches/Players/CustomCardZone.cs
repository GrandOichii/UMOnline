using NLua;
using UMCore.Matches.Players.Cards;

namespace UMCore.Matches.Players;

public class CustomCardZone : MatchCardCollection
{
    public CustomCardZone(
        Player owner,
        string name,
        LuaTable table
        ) : base(owner, name)
    {

    }

}