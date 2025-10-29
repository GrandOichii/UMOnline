using NLua;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Utility;

namespace UMCore.Matches;

public class FighterPredicateEffect(Fighter fighter, LuaTable table)
{
    private readonly Fighter _fighter = fighter;
    private readonly Effect _fighterPredicate = new(LuaUtility.TableGet<LuaFunction>(table, "fighterPred"));
    private readonly EffectCollection _effects = new(table);

    public bool Accepts(Fighter fighter)
    {
        return _fighterPredicate.ExecuteFighterCheck(_fighter, _fighter.Owner, fighter);
    }

    public void Execute(Fighter fighter, Player owner)
    {
        _effects.Execute(fighter, owner);
    }
}