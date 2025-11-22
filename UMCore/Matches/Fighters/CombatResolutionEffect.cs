using NLua;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;

namespace UMCore.Matches.Fighters;

// TODO this class shouldnt be in this directory

public class CombatResolutionEffect(Fighter fighter, LuaTable data)
{
    private readonly Fighter _fighter = fighter;
    private readonly EffectCollection _effects = new(fighter.Match, data);

    public void Execute(Player player)
    {
        _effects.Execute(new(_fighter), new(player));
    }
}