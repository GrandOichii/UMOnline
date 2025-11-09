using NLua;
using UMCore.Utility;

namespace UMCore.Matches;

public class OnMoveEffect(Fighter fighter, LuaTable table)
{
    private readonly Fighter _fighter = fighter;
    public string Text { get; } = LuaUtility.TableGet<string>(table, "text");
    private readonly LuaFunction _effectFunc = LuaUtility.TableGet<LuaFunction>(table, "effect");

    public void Execute(Fighter fighter, MapNode fromNode, MapNode? toNode)
    {
        _effectFunc.Call(
            MatchScripts.CreateArgs(_fighter, _fighter.Owner),
            fighter,
            fromNode,
            toNode
        );
    }
}