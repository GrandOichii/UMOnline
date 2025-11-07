using NLua;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Utility;

namespace UMCore.Matches;

public class FighterPredicateEffect(Fighter fighter, LuaTable table) : IHasText
{
    private readonly Fighter _fighter = fighter;
    private readonly Effect _fighterPredicate = new(LuaUtility.TableGet<LuaFunction>(table, "fighterPred"));
    private readonly EffectCollection _effects = new(table);

    public void Execute()
    {
        _effects.Execute(_fighter, _fighter.Owner);
    }

    public bool Accepts(Fighter fighter)
    {
        return _fighterPredicate.ExecuteFighterCheck(_fighter, _fighter.Owner, fighter);
    }

    public void Execute(Fighter fighter, Player owner)
    {
        _effects.Execute(fighter, owner);
    }

    public void Execute(Fighter fighter, Player owner, PlacedToken token)
    {
        _effects.Execute(fighter, owner, token);
    }

    public string GetText()
    {
        return _effects.GetText();
    }
}