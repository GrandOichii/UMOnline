
using NLua;
using UMCore.Utility;

namespace UMCore.Matches;

public class MovementNodeConnection(Fighter owner, LuaFunction func)
{
    public List<MapNode> GetConnectedNodesFor(Fighter fighter, MapNode node)
    {
        var returned = func.Call(MatchScripts.CreateArgs(owner, owner.Owner), fighter, node);
        var nodesRaw = LuaUtility.GetReturnAs<LuaTable>(returned);

        var result = LuaUtility.ParseTable<MapNode>(nodesRaw);
        return result;
    }
}